using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;
using DG.Tweening;
using Cinemachine;
using Rewriters.AbilitySystem;

namespace Rewriters.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ControllableBubble : Pickeable
    {
        #region Fields
        [SerializeField, ReadOnly, FoldoutGroup("Bubble Settings")] private Vector2 _initialPosition;
        [SerializeField] private ControllableBubbleStates _currentState = ControllableBubbleStates.Idle;
        public ControllableBubbleStates CurrentState => _currentState;
        [SerializeField, FoldoutGroup("Bubble Settings")] private float _bubbleLifeTime = 2f;
        [SerializeField, FoldoutGroup("Bubble Settings")] private float _jumpForce = 25f;
        [SerializeField, FoldoutGroup("Bubble Settings")] private float _bubbleSpeedMultiplier = 10f;
        [SerializeField, FoldoutGroup("Bubble Settings")] private float _secondsToStartMovingAfterPicked = 1f;

        [SerializeField, FoldoutGroup("Bubble Settings")] private float _playerRotationSpeedInsideBubble;
        [SerializeField, FoldoutGroup("Bubble Settings")] private Vector2 _playerScaleOnBubble;

        [InfoBox("Direction of the bubble in case the player is not holding any key.")]
        [SerializeField, FoldoutGroup("Bubble Settings")] private Vector2 _bubbleDirection;

        [SerializeField, FoldoutGroup("Effects")] private ParticleSystem _particleSystem;

        [SerializeField, FoldoutGroup("Cinemachine Impulse")] private CinemachineImpulseSource _impulseSource;

        [SerializeField, FoldoutGroup("Rotation")] private bool _canRotatePlayer = true;

        private Coroutine _stopBubbleAfterSeconds;

        private Vector3 GetPosition
        {
            get
            {
                if (Application.isPlaying)
                    return _initialPosition;

                return transform.position;
            }
        }
        #endregion
        protected override void Awake()
        {
            base.Awake();

            _initialPosition = transform.position;
            _currentState = ControllableBubbleStates.Idle;
        }

        protected override void OnPick(Character owner)
        {
            if (_currentState != ControllableBubbleStates.Idle)
                return;

            StopPlayerPhysics();

            Rigidbody.bodyType = RigidbodyType2D.Dynamic;

            _currentState = ControllableBubbleStates.WaitingToStartMoving;

            owner.GetComponent<Character>().SetCharacterState(CharacterStates.InBubbles);

            StartCoroutine(HandleBubbleBeforeStart_CO());
        }

        private void Update()
        {
            if (_currentState != ControllableBubbleStates.OnUse)
                return;

            if (PlayerInput.Instance.Jump)
            {
                HandleJump();
            }

            if (PlayerInput.Instance.Dash)
            {
                _canRotatePlayer = false;

                HandleDash();
            }

            if(_canRotatePlayer)
                Owner.transform.Rotate(new Vector3(0f, 0f, _playerRotationSpeedInsideBubble * Time.deltaTime));
        }

        private IEnumerator HandleBubbleBeforeStart_CO()
        {
            yield return new WaitForSeconds(_secondsToStartMovingAfterPicked);

            _currentState = ControllableBubbleStates.OnUse;

            MoveBubble();

            CaptureScreen();

            GenerateCinemachineImpulse();

            _stopBubbleAfterSeconds = StartCoroutine(StopBubbleAfterSeconds_CO());

            EnableParticles();
        }

        private IEnumerator StopBubbleAfterSeconds_CO()
        {
            yield return new WaitForSeconds(_bubbleLifeTime);

            ReanudatePlayerPhysics(false);
        }

        public virtual void EnableParticles()
        {
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Play();
        }

        public virtual void GenerateCinemachineImpulse()
        {
            _impulseSource.GenerateImpulse();
        }

        private void MoveBubble()
        {
            if (PlayerInput.Instance.Move.x != 0f || PlayerInput.Instance.Move.y != 0f)
            {
                Rigidbody.velocity = PlayerInput.Instance.Move * _bubbleSpeedMultiplier;
                return;
            }

            Rigidbody.velocity = _bubbleDirection * _bubbleSpeedMultiplier;
        }

        private void StopPlayerPhysics()
        {
            CharacterController2D ch2D = Owner.GetComponent<CharacterController2D>();
            ch2D.CanMove = false;
            ch2D.FlipProcess.Kill();

            Rigidbody2D playerRGB = Owner.GetComponent<Rigidbody2D>();
            playerRGB.bodyType = RigidbodyType2D.Kinematic;
            playerRGB.velocity = Vector2.zero;

            Collider2D playerCollider = Owner.GetComponent<Collider2D>();
            playerCollider.isTrigger = true;

            PlayerMovement playerMovement = Owner.GetComponent<PlayerMovement>();
            playerMovement.enabled = false;

            Owner.transform.SetParent(this.transform);
            Owner.transform.position = transform.position;

            Owner.transform.localScale = new Vector3(.5f * (ch2D.FacingRight ? 1f : -1f), .5f, 0f);
            Owner.transform.DOPunchScale(new Vector3(_playerScaleOnBubble.x, _playerScaleOnBubble.y, 0f), _secondsToStartMovingAfterPicked, 10, 1f);
        }

        protected virtual void CaptureScreen()
        {
            UIManager.Instance.Screenshot();
        }

        protected virtual void ResetBubble()
        {
            _currentState = ControllableBubbleStates.Returning;

            Rigidbody.bodyType = RigidbodyType2D.Static;
            Collider.isTrigger = true;

            transform.DOMove(_initialPosition, .5f).OnComplete(() =>
            {
                OnReachInitialPosition();
            });
        }

        protected virtual void OnReachInitialPosition()
        {
            _currentState = ControllableBubbleStates.Idle;
            Collider.isTrigger = false;
            _canRotatePlayer = true;
            _particleSystem.Stop();
        }

        protected virtual void HandleDash()
        {
            PlayerAbilityHandler abilityHandler = Owner.GetComponent<PlayerAbilityHandler>();
            AbilityHolder dashHolder = abilityHandler.GetAbility<Dash>();

            if (dashHolder.CurrentAbilityState != AbilityStates.ReadyToUse) return;

            if (_stopBubbleAfterSeconds != null) StopCoroutine(_stopBubbleAfterSeconds);

            ReanudatePlayerPhysics(false);
        }

        protected virtual void HandleJump()
        {
            if (_stopBubbleAfterSeconds != null)
                StopCoroutine(_stopBubbleAfterSeconds);

            ReanudatePlayerPhysics(true);
        }

        private void ReanudatePlayerPhysics(bool jump)
        {
            ResetBubble();

            CharacterController2D ch2D = Owner.GetComponent<CharacterController2D>();
            ch2D.CanMove = true;

            Rigidbody2D playerRGB = Owner.GetComponent<Rigidbody2D>();
            playerRGB.bodyType = RigidbodyType2D.Dynamic;
            playerRGB.velocity = Vector2.zero;

            Collider2D playerCollider = Owner.GetComponent<Collider2D>();
            playerCollider.isTrigger = false;

            PlayerMovement playerMovement = Owner.GetComponent<PlayerMovement>();
            playerMovement.enabled = true;

            Owner.GetComponent<Character>().SetCharacterState(CharacterStates.Jumping);

            Owner.transform.SetParent(null);

            Owner.transform.ResetRotation();

            Owner.transform.localScale = new Vector3(1f * (ch2D.FacingRight ? 1f : -1f), 1f, 0f);

            if (jump)
            {
                Owner.Rigidbody.velocity = Vector2.zero;
                Owner.gameObject.GetComponent<CharacterController2D>().Jump(new Vector2(0f, _jumpForce));
            }
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if(_currentState == ControllableBubbleStates.OnUse)
            {
                if (_stopBubbleAfterSeconds != null)
                    StopCoroutine(_stopBubbleAfterSeconds);

                ControllableBubble ctrBubble = collision.gameObject.GetComponent<ControllableBubble>();

                //In case we collide with another bubble.
                if (ctrBubble != null && ctrBubble.CurrentState == ControllableBubbleStates.Idle)
                {
                    ctrBubble.Pick(Owner);

                    this.ResetBubble();
                }
                else if (ctrBubble == null)
                {
                    ReanudatePlayerPhysics(false);
                }
            }

            base.OnCollisionEnter2D(collision);
        }

        private void OnDrawGizmosSelected()
        {
            float distance = _bubbleLifeTime * _bubbleSpeedMultiplier;

            Gizmos.color = Color.red;

            //Draws a line on the Up-Direction.
            Vector3 upDirection = GetPosition;
            upDirection.y += distance;
            Gizmos.DrawLine(GetPosition, upDirection);

            //Draws a line on the Down-Direction.
            Vector3 downDirection = GetPosition;
            downDirection.y -= distance;
            Gizmos.DrawLine(GetPosition, downDirection);

            //Draws a line on the Right-Direction.
            Vector3 rightDirection = GetPosition;
            rightDirection.x += distance;
            Gizmos.DrawLine(GetPosition, rightDirection);
            
            //Draws a line on the Left-Direction.
            Vector3 leftDirection = GetPosition;
            leftDirection.x -= distance;
            Gizmos.DrawLine(GetPosition, leftDirection);

        }
    }

    public enum ControllableBubbleStates
    {
        OnUse,
        WaitingToStartMoving,
        Returning,
        Idle
    }
}