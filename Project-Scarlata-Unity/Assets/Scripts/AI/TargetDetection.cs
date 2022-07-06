using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Rewriters.AI
{
    [ExecuteInEditMode]
    public class TargetDetection : MonoBehaviour
    {
        [SerializeField] private TargetDetectionType _type;
        public TargetDetectionType Type => _type;

        [SerializeField, ] private Transform _startPosition;
        [SerializeField] private LayerMask _targetsLayers;

        //Box Bounds
        [SerializeField, ShowIf("_type", TargetDetectionType.Box)] private Bounds _bounds;
        public Bounds Bounds { get => _bounds; set => _bounds = value; }

        //Sphere Size
        [SerializeField, ShowIf("_type", TargetDetectionType.Sphere)] private float _radius;
        public float Radius { get => _radius; set => _radius = value; }
        [SerializeField] private Vector3 _center;
        public Vector3 SphereCenter => _center;

        [SerializeField] private Vector3 _targetPosition;
        public Vector3 TargetPosition { get => _targetPosition; set => _targetPosition = value; }

        public virtual void Update()
        {
            if(Type == TargetDetectionType.Sphere)
                _center = _targetPosition;
            
        }

        private void DrawBox()
        {
            
        }

        private void DrawSphere()
        {

        }

        private void DrawLine()
        {
            
        }
    }

    public enum TargetDetectionType
    {
        Box,
        Sphere,
        Line
    }
}