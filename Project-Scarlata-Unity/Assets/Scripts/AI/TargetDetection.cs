using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PathBerserker2d;

namespace Rewriters.AI
{
    [RequireComponent(typeof(NavAgent))]
    [RequireComponent(typeof(TransformBasedMovement))]
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

        [SerializeField] private bool _returnToInitialPositionWhenTargetIsOut = false;

        public bool ReturnToInitialPosition => _returnToInitialPositionWhenTargetIsOut;

        private Vector3 _initialPosition;
        public Vector3 InitialPosition => _initialPosition;

        [SerializeField] private float _reachDistance = .5f;
        public float ReachDistance => _reachDistance;
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

        #region Stop Character
        private Coroutine _handleStop;
        #endregion

        #region Animator
        public Animator Animator;
        #endregion

        #region Cooldown
        [SerializeField] private float _cooldown;

        private Coroutine _handleCooldown;
        #endregion

        #region StateMachine
        public StateMachine<TargetDetection> StateMachine;
        #endregion

        private void Awake()
        {
            _initialPosition = transform.position;

            StateMachine = new StateMachine<TargetDetection>(this);
            StateMachine.AddState<IdleState>();
            StateMachine.AddState<FollowState>();
            StateMachine.AddState<ReturningState>();
            StateMachine.AddState<StoppedState>();

            StateMachine.SetState<IdleState>();
        }

        private void Update()
        {
            StateMachine.CurrentState.OnUpdate(this);
        }

        /// <summary>
        /// Moves the target to the specified position.
        /// </summary>
        /// <param name="position"></param>
        public void MoveTo(Vector3 position)
        {
            if (ReachedDistance(position))
                return;

            if (!CanMove)
                return;

            NavAgent.PathTo(position);

            _handleCooldown = StartCoroutine(HandleCooldown_CO());
        }

        /// <summary>
        /// Cooldown to repath and follow the target.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Stops the agent on it's exact position.
        /// </summary>
        public void StopAgent() => StateMachine.SetState<StoppedState>();

        /// <summary>
        /// Stops the agent for few seconds on it's exact position and then return to Idle State.
        /// </summary>
        /// <param name="seconds"></param>
        public virtual void StopAgent(float seconds)
        {
            if (_handleStop != null)
                StopCoroutine(_handleStop);

            _handleStop = StartCoroutine(StopAgent_CO(seconds));
        }

        protected virtual IEnumerator StopAgent_CO(float seconds)
        {
            StateMachine.SetState<StoppedState>();

            yield return new WaitForSeconds(seconds);

            StateMachine.SetState<IdleState>();
        }

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

        /// <summary>
        /// Checks if reached the given distance.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool ReachedDistance(Vector3 position)
        {
            bool reachedDistance = Vector3.Distance(transform.position, position) < _reachDistance;

            if (reachedDistance)
                NavAgent.Stop();

            return reachedDistance;
        }
    }

    public enum TargetDetectionType
    {
        Box,
        Sphere,
        Line
    }
}