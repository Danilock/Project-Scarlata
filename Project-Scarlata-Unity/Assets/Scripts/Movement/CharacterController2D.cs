using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;
using Rewriters.Player;

namespace Rewriters
{
	public class CharacterController2D : MonoBehaviour
	{
		#region Movement and Jump Fields

		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private float m_JumpForce = 400f;                            // Amount of force added when the player jumps.
		[SerializeField] private bool m_isInAirDueToWallJump;
		private Vector3 _lastScale;
		private float CalculateJumpForceDirection
		{
			get
			{
				if (m_FacingRight)
				{
					return -_wallJumpForce;
				}

				return _wallJumpForce;
			}
		}

		private bool _wasGrounded = false;
		[Range(0, 1)][SerializeField, FoldoutGroup("Movement and Jump Fields")] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
		[SerializeField, Range(0, 1), FoldoutGroup("Movement and Jump Fields")] private float m_speed = 1f;
		private float _initialSpeed;
		[SerializeField, Range(0, 1), FoldoutGroup("Movement and Jump Fields")] private float m_speedWhenNotGrounded;
		[SerializeField, Range(0, 1), FoldoutGroup("Movement and Jump Fields")] private float _timeToWaitToSetAirSpeedAfterJump = .2f;
		private float _initialSpeedMultiplier;

		[SerializeField, Range(1, 100), FoldoutGroup("Movement and Jump Fields")]
		private float _sprintSpeed;
		public float CrouchSpeed { get { return m_CrouchSpeed; } set { m_CrouchSpeed = value; } }
		[Range(0, .3f)][SerializeField, FoldoutGroup("Movement and Jump Fields")] private float m_MovementSmoothing = .05f; // How much to smooth out the movement
		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private bool m_AirControl = false;                           // Whether or not a player can steer while jumping;
		public bool AirControl { get { return m_AirControl; } set { m_AirControl = value; } }
		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private LayerMask m_WhatIsGround;                            // A mask determining what is ground to the character
		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private Transform m_GroundCheck;                         // A position marking where to check if the player is grounded.
		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private Transform m_CeilingCheck;                            // A position marking where to check for ceilings
		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private Collider2D m_CrouchDisableCollider;              // A collider that will be disabled when crouching

		[SerializeField, FoldoutGroup("Movement and Jump Fields")]
		private float _jumpDeadTime = .2f;

		[SerializeField, FoldoutGroup("Movement and Jump Fields"), Range(1.1f, 100f)]
		private float _softJumpMultiplier = 1.5f;

		[SerializeField, FoldoutGroup("Movement and Jump Fields")]
		private bool _ignoreXForceOnJump = false;

		private bool _canMove = true;
		public bool CanMove
		{
			get
			{
				return _canMove;
			}
			set
			{
				_canMove = value;
			}
		}

		[SerializeField, FoldoutGroup("Movement and Jump Fields")] private float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
		[SerializeField] private bool m_Grounded;            // Whether or not the player is grounded.
		public bool IsGrounded
		{
			get
			{
				return m_Grounded;
			}
		}
		[Header("Events")]
		[Space]

		[FoldoutGroup("Events")] public UnityEvent OnLandEvent;
		[FoldoutGroup("Events")] public UnityEvent OnGroundEvent;

		const float k_CeilingRadius = .2f;

		private Vector3 m_Velocity = Vector3.zero;


		[System.Serializable]
		public class BoolEvent : UnityEvent<bool> { }

		[FoldoutGroup("Events")] public BoolEvent OnCrouchEvent;
		private bool m_wasCrouching = false;
		#endregion

		#region Other

		// Radius of the overlap circle to determine if the player can stand up
		public Rigidbody2D Rigidbody;
		[SerializeField] private bool m_FacingRight = true;  // For determining which way the player is currently facing.
		public bool FacingRight { get => m_FacingRight; set => m_FacingRight = value; }

		public Collider2D[] Colliders { get; private set; }

		private bool _jumpCoroutineStarted;

		#endregion

		#region Wall Check
		[SerializeField, FoldoutGroup("Wall Check")] private bool m_stopYvelocityOnDetectWall = false;
		[SerializeField, FoldoutGroup("Wall Check")] private LayerMask m_wallLayer;
		[SerializeField, FoldoutGroup("Wall Check")] private Vector2 m_wallCheckOffset;
		[SerializeField, FoldoutGroup("Wall Check")] private float m_wallCheckSize = 4f;
		[SerializeField, FoldoutGroup("Wall Check")] private bool m_hitWall;

		[SerializeField, FoldoutGroup("Wall Check")]
		private float m_rotationDurationOnWallJump = .1f;

		[SerializeField, FoldoutGroup("Wall Check")]
		private float _movementTimeOffAfterWallJump = .2f;

		[SerializeField, Sirenix.OdinInspector.ReadOnly, FoldoutGroup("Wall Check")] private float _initialGravity;

		[SerializeField, FoldoutGroup("Wall Check")] private float _gravityOnceDetectingWall = 7f;

		[SerializeField, ReadOnly] private bool _isWallClimbing = false;

		[SerializeField, FoldoutGroup("Wall Check"), Range(0, 20)] private float _wallJumpForce;

		[SerializeField, FoldoutGroup("Wall Check"), ReadOnly] private bool m_wasOnWall = false;
		#endregion

		#region Animator
		[SerializeField] private Animator _animator;

		private static readonly int _hashSpeed = Animator.StringToHash("Speed");
		private static readonly int _hashJump = Animator.StringToHash("Jump");
		private static readonly int _hashGrounded = Animator.StringToHash("IsGrounded");
		private static readonly int _hashYvelocity = Animator.StringToHash("Yvelocity");
		private static readonly int _hashXvelocity = Animator.StringToHash("Xvelocity");
		private static readonly int _hashWallClimb = Animator.StringToHash("WallClimb");
		private static readonly int _hashWallClimbDirection = Animator.StringToHash("WallClimbDirection");
		private static readonly int _hashSprint = Animator.StringToHash("Sprint");
		private static readonly int _hashWasGrounded = Animator.StringToHash("WasGrounded");
		#endregion

		#region Body

		[FoldoutGroup("Body"), SerializeField] private Transform _body;
		[FoldoutGroup("Body"), SerializeField] private float _YscaleOnceCrouch = 5.9573f;
		[FoldoutGroup("Body"), SerializeField] private float _duration = .3f;
		private Vector3 _bodyStartSize;

		#endregion

		private PlayerInput _inputManager;

		#region Unity Methods

		private void Awake()
		{
			Rigidbody = GetComponent<Rigidbody2D>();

			if (OnLandEvent == null)
				OnLandEvent = new UnityEvent();

			if (OnCrouchEvent == null)
				OnCrouchEvent = new BoolEvent();

			if (_animator == null)
				_animator = GetComponent<Animator>();

			_bodyStartSize = _body.transform.localScale;
			_initialGravity = Rigidbody.gravityScale;
			_initialSpeedMultiplier = m_speed;
			_inputManager = GetComponent<PlayerInput>();
			_lastScale = transform.localScale;
			_initialSpeed = m_speed;
		}

		private void FixedUpdate()
		{
			_wasGrounded = m_Grounded;
			m_wasOnWall = m_hitWall;

			m_Grounded = false;
			m_hitWall = false;

			// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
			// This can be done using layers instead but Sample Assets will not overwrite your project settings.

			if (m_GroundCheck == null)
				return;
			Colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

			for (int i = 0; i < Colliders.Length; i++)
			{
				if (Colliders[i].gameObject != this.gameObject)
				{
					m_Grounded = true;
					m_isInAirDueToWallJump = false;
					_isWallClimbing = false;
					OnGroundEvent.Invoke();
					if (!_wasGrounded)
					{
						m_speed = _initialSpeed;
						OnLandEvent.Invoke();
						//_animator.SetBool(_hashJump, false);
					}
				}
			}

			m_hitWall = Physics2D.Linecast(CalculateWallCheckStartPosition(), CalculateWallCheckEndPosition(), m_wallLayer);

			if (!m_hitWall)
			{
				Rigidbody.gravityScale = _initialGravity;
				_isWallClimbing = false;
			}

			if (m_hitWall)
			{
				m_isInAirDueToWallJump = false;

                if (!m_wasOnWall && m_stopYvelocityOnDetectWall)
                {
					Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0f);
                }
			}
		}

		private void Update()
		{
			if (m_hitWall && !m_Grounded)
			{
				Rigidbody.gravityScale = _gravityOnceDetectingWall;

				//_animator.SetBool(_hashJump, false);
				//_animator.SetFloat(_hashWallClimbDirection, m_FacingRight ? 1 : -1);

				_isWallClimbing = true;

				if (_inputManager.JumpWasPressedThisFrame && !m_isInAirDueToWallJump)
				{
					Jump(new Vector2(CalculateJumpForceDirection, _wallJumpForce));
					Flip(m_rotationDurationOnWallJump);
					m_isInAirDueToWallJump = true;
					StartCoroutine(HandleWallJump());
					_isWallClimbing = false;
				}
			}

			//_animator.SetBool(_hashGrounded, m_Grounded);
			//_animator.SetFloat(_hashYvelocity, Rigidbody.velocity.y);
			//_animator.SetFloat(_hashXvelocity, Rigidbody.velocity.x);
			//_animator.SetBool(_hashWallClimb, m_hitWall && !m_Grounded);
			//_animator.SetBool(_hashWasGrounded, _wasGrounded);
		}
		#endregion

		public void Move(float move, bool crouch, bool jump, bool sprint)
		{
			if (!_canMove)
				return;

			//if (m_isInAirDueToWallJump)
			//return;

			if (m_Grounded && m_hitWall)
			{
				if (m_FacingRight && move > 0)
				{
					//_animator.SetFloat(_hashSpeed, 0f);
					return;
				}
				else if (!m_FacingRight && move < 0)
				{
					//_animator.SetFloat(_hashSpeed, 0f);
					return;
				}
			}
			//if(move == transform.localScale.x && CollidedWithWall()){
			//Debug.Log("Wall");
			//return;
			//}

			// If crouching, check to see if the character can stand up
			if (!crouch)
			{
				// If the character has a ceiling preventing them from standing up, keep them crouching
				if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
				{
					crouch = true;
				}
			}

			//only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl)
			{
				// If crouching
				if (crouch && m_Grounded)
				{
					if (!m_wasCrouching)
					{
						m_wasCrouching = true;
						OnCrouchEvent.Invoke(true);
					}

					// Reduce the speed by the crouchSpeed multiplier
					move *= m_CrouchSpeed;

					// Disable one of the colliders when crouching
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = false;
				}
				else
				{
					// Enable the collider when not crouching
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = true;

					if (m_wasCrouching)
					{
						m_wasCrouching = false;
						OnCrouchEvent.Invoke(false);
					}
				}

				if (_isWallClimbing && ((m_FacingRight && move > 0) || (!m_FacingRight && move < 0)))
					return;

				if (Mathf.Abs(move) > 0)
				{
					// Move the character by finding the target velocity
					Vector3 targetVelocity = new Vector2((move * 10f * m_speed), Rigidbody.velocity.y);
					// And then smoothing it out and applying it to the character
					Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, targetVelocity, ref m_Velocity,
						m_MovementSmoothing);
				}
                else if(move == 0f && m_Grounded)
                {
					Rigidbody.velocity = new Vector2(0f, Rigidbody.velocity.y);
                }

				//_animator.SetFloat(_hashSpeed, Mathf.Abs(move));
				//_animator.SetBool(_hashSprint, sprint && move != 0);

				// If the input is moving the player right and the player is facing left...
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip(.3f);
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.

					Flip(.3f);
				}
			}
			// If the player should jump...
			if (m_Grounded && jump && !m_isInAirDueToWallJump && !_jumpCoroutineStarted)
			{
				// Add a vertical force to the player.
				m_Grounded = false;
				_jumpCoroutineStarted = true;
				StartCoroutine(HandleSoftJump());
			}
		}

		private IEnumerator HandleWallJump()
		{
			_canMove = false;
			yield return new WaitForSeconds(_movementTimeOffAfterWallJump);
			_canMove = true;
		}

		private IEnumerator HandleSoftJump()
		{
			yield return new WaitForSeconds(_jumpDeadTime);
			//_animator.SetBool(_hashJump, true);

			if (_inputManager.JumpWasPressedThisFrame)
			{
				Jump(new Vector2(Rigidbody.velocity.x, m_JumpForce));
			}
			else
			{
				Jump(new Vector2(Rigidbody.velocity.x, m_JumpForce / _softJumpMultiplier));
			}

			_jumpCoroutineStarted = false;

			yield return new WaitForSeconds(.1f);

			if(m_hitWall)
				Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0f);


		}

		private void Flip(float flipDuration)
		{
			m_FacingRight = !m_FacingRight;

			Vector3 theScale = _lastScale;
			theScale.x *= -1;

			_lastScale = theScale;

			transform.DOScale(theScale, flipDuration);
		}

		#region  Public Methods
		public void Jump(Vector2 force)
		{
			Rigidbody.velocity = Vector2.zero;

			Rigidbody.velocity = new Vector2(_ignoreXForceOnJump ? 0f : Rigidbody.velocity.x, 0f);
			Rigidbody.AddForce(new Vector2(_ignoreXForceOnJump ? 0f : force.x, force.y), ForceMode2D.Impulse);

			StartCoroutine(HandleSpeedOnAir());
		}

		private IEnumerator HandleSpeedOnAir()
        {
			yield return new WaitForSeconds(_timeToWaitToSetAirSpeedAfterJump);

			m_speed = m_speedWhenNotGrounded;
        }

		public bool IsMoving()
		{
			return
			Mathf.Abs(m_Velocity.x) > .1f ||
			Mathf.Abs(m_Velocity.y) > .1f;
		}

		///<Summary>
		///Totally grounded means the player is in the floor and with zero velocity in y axis
		///</Summary> 
		public bool IsTotallyGrounded()
		{
			if (m_Grounded && Mathf.Abs(Rigidbody.velocity.y) < .1f)
				return true;
			else if (m_Grounded && Mathf.Abs(Rigidbody.velocity.y) > .1f)
				return true;

			return false;
		}
		#endregion

		private void OnDrawGizmos()
		{
			if (m_GroundCheck == null)
				return;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(m_GroundCheck.position, k_GroundedRadius);

			Gizmos.color = Color.red;
			Gizmos.DrawLine(CalculateWallCheckStartPosition(), CalculateWallCheckEndPosition());
		}

		private Vector2 CalculateWallCheckEndPosition()
		{
			return new Vector2(CalculateWallCheckStartPosition().x + m_wallCheckSize * Mathf.Sign(transform.localScale.x), CalculateWallCheckStartPosition().y); ;
		}

		private Vector2 CalculateWallCheckStartPosition()
		{
			return (Vector2) transform.position + m_wallCheckOffset;
		}
	}
}