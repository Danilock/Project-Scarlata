using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Rewriters.AI
{
    [ExecuteInEditMode]
    public class TargetDetection : MonoBehaviour
    {
        #region Settings
        [SerializeField] private TargetDetectionType _type;
        public TargetDetectionType Type => _type;

        [SerializeField] private LayerMask _targetsLayers;
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


        public virtual bool IsDetectingATarget()
        {
            Collider2D col;

            if (Type == TargetDetectionType.Box)
            {
                col = Physics2D.OverlapBox(transform.position + Offset, Bounds.size, 0f, _targetsLayers);

                Target = col?.transform;

                return col != null;
            }

            if (Type == TargetDetectionType.Sphere)
            {
                col = Physics2D.OverlapCircle(transform.position + Offset, _radius, _targetsLayers);

                Target = col?.transform;

                return col != null;
            }

            return false;
        }
    }

    public enum TargetDetectionType
    {
        Box,
        Sphere,
        Line
    }
}