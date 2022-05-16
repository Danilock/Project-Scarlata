using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PathBerserker2d
{
    /// <summary>
    /// A collection of line segments to traverse on.
    /// </summary>
    /// <remarks>
    /// NavSurfaces are independent collections of segments. At runtime their data gets added to the pathfinder.
    /// You can load and unload NavSurfaces at runtime. This allows you to setup Prefabs with navigation data and stream them in at runtime.
    /// Loading and unloading is as simple as enabling and disabling a NavSurface script.
    /// ## Baking
    /// The bake process **only consideres colliders that are children of the NavSurface**.
    /// This also extends to the clearance calculation of segment cells.
    /// Baking is currently limited to editor mode only. You can't bake at runtime.
    /// ## Transformations
    /// All baked postion data is relative to the current NavSurface transformation.
    /// Segments in calculated paths remain relative to the NavSurface of origin. 
    /// If you pathfind on a NavSurface and then move it, the path will reflect that transformation, without any extra calculations.
    ///
    /// See also \ref cc_navagent "Core concepts: NavAgent".
    /// <remarks/>
    [ScriptExecutionOrder(-50)]
    [AddComponentMenu("PathBerserker2d/Nav Surface")]
    public class NavSurface : MonoBehaviour, INavSegmentCreationParamProvider
    {
        internal const int CurrentBakeVersion = 2;

        public Rect WorldBounds => Geometry.EnlargeRect(Geometry.TransformBoundingRect(localBoundingRect, LocalToWorldMatrix), PathBerserker2dSettings.PointMappingDistance);

        public float MaxClearance => maxClearance;

        public float MinClearance => minClearance;

        public float CellSize => cellSize;

        public LayerMask ColliderMask => includedColliders;

        /// <summary>
        /// Length of all segments combined
        /// </summary>
        public float TotalLineLength => totalLineLength;

        /// <summary>
        /// Segments exceeding this angle are removed from the bake output. Should equal the highest slope angle of your NavAgents.
        /// </summary>
        public float MaxSlopeAngle => maxSlopeAngle;

        /// <summary>
        /// Parameter for the line simplifier (Ramer-Douglas-Peucker). Higher values reduce overall segment count at the expense of fitting the original collider shape.
        /// </summary>
        public float SmallestDistanceYouCareAbout => smallestDistanceYouCareAbout;

        /// <summary>
        /// Segments shorter than this will be removed from the bake output.
        /// </summary>
        public float MinSegmentLength => minSegmentLength;

        /// <summary>
        /// Gets fired after a BakeJob initiated by a call to Bake() completes. 
        /// </summary>
        public event Action OnBakingCompleted;

        internal List<NavSegment> NavSegments => navSegments;
        internal NavSurfaceBakeJob BakeJob { get; private set; }
        internal bool hasDataChanged;
        internal int BakeVersion => bakeVersion;

        [Header("Bake Settings")]
        [Tooltip("Maximum height that gets checked for potential obstructions. Should equal the height of your largest NavAgent.")]
        [SerializeField]
        float maxClearance = 1.8f;

        [Tooltip("Parts of segments with less unobstructed space will be erased. Should equal the height of your smallest NavAgent.")]
        [SerializeField]
        float minClearance = 0.1f;

        [Tooltip("Size of a single segment part. Smaller numbers increase the accuracy of obstruction calculations at the expense of both bake and runtime performance.")]
        [SerializeField]
        float cellSize = 0.1f;

        [Tooltip("Colliders to consider for the bake process.")]
        [SerializeField]
        LayerMask includedColliders = ~0;


        [Tooltip("Use only colliders from gameobjects marked static.")]
        [SerializeField]
        bool onlyStaticColliders = false;

        [Tooltip("Segments exceeding this angle are removed from the bake output. Should equal the highest slope angle of your NavAgents.")]
        [SerializeField]
        [Range(0, 180)]
        float maxSlopeAngle = 180f;

        [Tooltip("Parameter for the line simplifier (Ramer-Douglas-Peucker). Higher values reduce overall segment count at the expense of fitting the original collider shape.")]
        [SerializeField]
        float smallestDistanceYouCareAbout = 0.1f;

        [Tooltip("Segments shorter than this will be removed from the bake output.")]
        [SerializeField]
        float minSegmentLength = 0.1f;

        [SerializeField]
        private List<NavSegment> navSegments = new List<NavSegment>();

        [SerializeField, HideInInspector]
        private Rect localBoundingRect;
        [SerializeField, HideInInspector]
        private float totalLineLength;
        [SerializeField, HideInInspector]
        private int bakeVersion = 0;

        #region Unity_Methods
        private void OnEnable()
        {
            if (navSegments != null && navSegments.Count > 0)
                PBWorld.NavGraph.AddNavSurface(this);
        }

        private void OnDisable()
        {
            PBWorld.NavGraph.RemoveNavSurface(this);
        }

        private void OnValidate()
        {
            if (minClearance <= 0)
                minClearance = 0.1f;
            if (maxClearance <= minClearance)
                maxClearance = minClearance + 0.1f;
            if (cellSize <= 0)
                cellSize = 0.1f;

            BakeJob = new NavSurfaceBakeJob(this);
            hasDataChanged = true;
        }

        #endregion

        public Vector2 LocalToWorld(Vector2 pos)
        {
            return LocalToWorldMatrix.MultiplyPoint3x4(pos);
        }

        public Matrix4x4 LocalToWorldMatrix
        {
            get
            {
                return Matrix4x4.TRS(transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z), Vector3.one);
            }
        }

        public Vector2 WorldToLocal(Vector2 pos)
        {
            return WorldToLocalMatrix.MultiplyPoint3x4(pos);
        }

        public Matrix4x4 WorldToLocalMatrix => LocalToWorldMatrix.inverse;

        /// <summary>
        /// Updates NavSurface baked data. Baking largely runs in a different thread. This function should be run as a Coroutine. NavSurface will be removed from World first and added back, when baking is completed.
        /// Calling this function before the previous bake job completed, will abort the previous job.
        /// MAY NOT WORK ON EVERY PLATFORM (like WebGL). Uses threading and build target needs to support that.
        /// </summary>
        public IEnumerator Bake()
        {
            PBWorld.NavGraph.RemoveNavSurface(this);
            StartBakeJob();

            while (!BakeJob.IsFinished)
            {
                yield return null;
            }
            UpdateInternalData(BakeJob.navSegments, BakeJob.bounds);
            PBWorld.NavGraph.AddNavSurface(this);

            OnBakingCompleted?.Invoke();
        }

        internal void StartBakeJob()
        {
            if (BakeJob == null)
                BakeJob = new NavSurfaceBakeJob(this);
            else
                BakeJob.AbortJoin();

            var subtractors = GetComponentsInChildren<NavSegmentSubstractor>();
            Tuple<Rect, Vector2>[] substractorRects = new Tuple<Rect, Vector2>[subtractors.Length];
            for (int i = 0; i < subtractors.Length; i++)
            {
                var rT = subtractors[i].GetComponent<RectTransform>();
                var rect = rT.rect;
                Vector2 scaleFactor = rT.lossyScale * rect.size * 0.5f;
                Vector2 center = rect.center;

                rect.min = center - scaleFactor + (Vector2)rT.position;
                rect.max = center + scaleFactor + (Vector2)rT.position;

                substractorRects[i] = new Tuple<Rect, Vector2>(rect, new Vector2(subtractors[i].fromAngle, subtractors[i].toAngle));
            }

            var filter = new ColliderLayerFilter(includedColliders, onlyStaticColliders);
            var allColliders = filter.Filter(this.GetComponentsInChildren<Collider2D>()).ToArray();

            var it = new IntersectionTester(this, WorldToLocalMatrix);
            Polygon[][] polygons = new Polygon[allColliders.Length][];
            for (int i = 0; i < allColliders.Length; i++)
            {
                var col = allColliders[i];
                polygons[i] = it.ColliderToPolygon(col);
            }

            BakeJob.Start(polygons, it, substractorRects, WorldToLocalMatrix);
        }

        internal NavSegment GetSegment(int index)
        {
            return navSegments[index];
        }

        internal void UpdateInternalData(List<NavSegment> segments, Rect bounds)
        {
            if (segments == null)
            {
                Debug.LogError("Updating NavSurface failed. Got null as segments");
                return;
            }

            this.localBoundingRect = Geometry.TransformBoundingRect(bounds, WorldToLocalMatrix);

            this.totalLineLength = 0;
            foreach (var seg in segments)
                totalLineLength += seg.Length;

            this.navSegments = segments;
            bakeVersion = CurrentBakeVersion;
            hasDataChanged = true;
        }
    }
}