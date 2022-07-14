using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using PathBerserker2d;

namespace Rewriters.AI
{
    public class AIAgent : MonoBehaviour
    {
        #region Settings
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
        public StateMachine<AIAgent> StateMachine;
        #endregion

        #region Target Detection
        public TargetDetection TargetDetection;
        #endregion

        #region Ground
        [SerializeField, FoldoutGroup("Ground Settings")] protected float GroundRadius;
        [SerializeField, FoldoutGroup("Ground Settings")] protected Vector2 GroundOffset;
        [SerializeField, FoldoutGroup("Ground Settings")] protected LayerMask GroundLayers;
        #endregion

        private void Awake()
        {
            _initialPosition = transform.position;
            Animator = GetComponent<Animator>();
            _navAgent = GetComponent<NavAgent>();

            StateMachine = new StateMachine<AIAgent>(this);
            StateMachine.AddState<IdleState>();
            StateMachine.AddState<FollowState>();
            StateMachine.AddState<ReturningState>();
            StateMachine.AddState<StoppedState>();

            StateMachine.SetState<IdleState>();
        }

        private void Update()
        {
            StateMachine.CurrentState.OnUpdate(this);

            Debug.Log(StateMachine.CurrentState);
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

        public void ForceStopAgent()
        {
            if (_handleStop != null)
                StopCoroutine(_handleStop);

            StateMachine.SetState<StoppedState>();
        }

        protected virtual IEnumerator StopAgent_CO(float seconds)
        {
            StateMachine.SetState<StoppedState>();

            yield return new WaitForSeconds(seconds);

            StateMachine.SetState<IdleState>();
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

        public bool IsGrounded()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position + (Vector3)GroundOffset, GroundRadius,  GroundLayers);

            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != this.gameObject)
                    return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _reachDistance);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position + (Vector3)GroundOffset, GroundRadius);
        }
    }
}