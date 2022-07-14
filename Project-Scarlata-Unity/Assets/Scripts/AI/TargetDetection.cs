using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PathBerserker2d;

namespace Rewriters.AI
{
    public class TargetDetection : MonoBehaviour
    {
        #region Settings
        [SerializeField] private TargetDetectionType _type;
        public TargetDetectionType Type => _type;

        [SerializeField] private LayerMask _targetsLayers;

        // Checks the direction of the object.
        public bool ConsiderObjectDirection;

        public Transform Body;

        public Vector3 GetPoint
        {
            get
            {
                if (Body == null)
                    return transform.position + Offset;

                if (ConsiderObjectDirection)
                    Offset.x = Mathf.Abs(Offset.x) * Mathf.Sign(Body.transform.localScale.x);

                Vector3 point = Body.position + Offset;

                return point;
            }
        }
        #endregion

        #region Box Bounds
        [SerializeField] private Bounds _bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(1f, 1f));
        public Bounds Bounds { get => _bounds; set => _bounds = value; }
        #endregion

        #region Sphere and Target Position
        [SerializeField] private float _radius = 1f;
        public float Radius { get => _radius; set => _radius = value; }
        [SerializeField] private Vector3 _sphereCenter;
        public Vector3 SphereCenter { get => _sphereCenter; set => _sphereCenter = value; }
        #endregion

        #region Target
        public Transform Target
        {
            get;
            private set;
        }
        #endregion

        #region Offset
        public Vector3 Offset;
        #endregion

        /// <summary>
        /// Checks if is detecting any target.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDetectingATarget()
        {
            Collider2D col;
            bool isDetectingTarget = false;

            if (Type == TargetDetectionType.Box)
            {
                col = Physics2D.OverlapBox(GetPoint, Bounds.size, 0f, _targetsLayers);

                Target = col?.transform;

                isDetectingTarget = col != null;
            }

            if (Type == TargetDetectionType.Sphere)
            {
                col = Physics2D.OverlapCircle(GetPoint, _radius, _targetsLayers);

                Target = col?.transform;

                isDetectingTarget = col != null;
            }

            return isDetectingTarget;
        }
    }

    public enum TargetDetectionType
    {
        Box,
        Sphere,
        Line
    }
}