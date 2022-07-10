using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PathBerserker2d;

namespace Rewriters.AI
{
    [RequireComponent(typeof(NavAgent))]
    public class TargetDetection : MonoBehaviour
    {
        #region Settings
        [SerializeField] private TargetDetectionType _type;
        public TargetDetectionType Type => _type;

        [SerializeField] private LayerMask _targetsLayers;

        [SerializeField] private bool _canMove;

        public bool CanMove { get => _canMove; set => _canMove = value; }

        [SerializeField] private NavAgent _navAgent;

        public NavAgent NavAgent => _navAgent;

        [SerializeField] private float _cooldown;

        private Coroutine _handleCooldown;

        [SerializeField] private bool _returnToInitialPositionWhenTargetIsOut = false;

        public bool ReturnToInitialPosition => _returnToInitialPositionWhenTargetIsOut;

        private Vector3 _initialPosition;
        public Vector3 InitialPosition => _initialPosition;
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

        #region
        public StateMachine<TargetDetection> StateMachine;
        #endregion

        private void Awake()
        {
            _initialPosition = transform.position;

            StateMachine = new StateMachine<TargetDetection>(this);
            StateMachine.AddState<IdleState>();
            StateMachine.AddState<FollowState>();
            StateMachine.AddState<ReturningState>();

            StateMachine.SetState<IdleState>();
        }

        private void Update()
        {
            StateMachine.CurrentState.OnUpdate(this);

            Debug.Log(StateMachine.CurrentState.ToString());
        }

        public void MoveTo(Vector3 position)
        {
            if (!CanMove)
                return;

            NavAgent.PathTo(position);

            _handleCooldown = StartCoroutine(HandleCooldown_CO());
        }

        private IEnumerator HandleCooldown_CO()
        {
            CanMove = false;

            yield return new WaitForSeconds(_cooldown);

            CanMove = true;
        }

        /// <summary>
        /// Stops the coroutine handling the cooldown. By disabling it, the CanMove variable should be stablished manually.
        /// </summary>
        public void StopCooldownHandler()
        {
            StopCoroutine(_handleCooldown);
        }

        public virtual bool IsDetectingATarget()
        {
            Collider2D col;
            bool isDetectingTarget = false;

            if (Type == TargetDetectionType.Box)
            {
                col = Physics2D.OverlapBox(transform.position + Offset, Bounds.size, 0f, _targetsLayers);

                Target = col?.transform;

                isDetectingTarget = col != null;
            }

            if (Type == TargetDetectionType.Sphere)
            {
                col = Physics2D.OverlapCircle(transform.position + Offset, _radius, _targetsLayers);

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