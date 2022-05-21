using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField, FoldoutGroup("Movement Settings")] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[Range(0, 1)][SerializeField, FoldoutGroup("Movement Settings")] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[SerializeField, Range(0, 1), FoldoutGroup("Movement Settings")] private float m_SpeedMultiplier;
	public float CrouchSpeed { get { return m_CrouchSpeed; } set { m_CrouchSpeed = value; } }
	[Range(0, .3f)][SerializeField, FoldoutGroup("Movement Settings")] private float m_MovementSmoothing = .05f;   // How much to smooth out the movement
	[SerializeField, FoldoutGroup("Movement Settings")] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	public bool AirControl { get { return m_AirControl; } set { m_AirControl = value; } }
	[SerializeField, FoldoutGroup("Ground Settings")] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField, FoldoutGroup("Ground Settings")] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField, FoldoutGroup("Ceiling Settings")] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	[SerializeField, FoldoutGroup("Ground Settings")] private float k_GroundedRadius = .1f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	public bool IsGrounded
	{
		get
		{
			return m_Grounded;
		}
	}

	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	public Rigidbody2D Rigidbody;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	public bool FacingRight { get => m_FacingRight; set => m_FacingRight = value; }
	private Vector3 m_Velocity = Vector3.zero;

	[FoldoutGroup("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable, FoldoutGroup("Events")]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private bool CanMove = true;

	public Collider2D[] Colliders { get; private set; }

	private Vector3 m_lastScale;

	
	[SerializeField, FoldoutGroup("Wall Check")] private LayerMask m_wallLayer;
	[SerializeField, FoldoutGroup("Wall Check")] private float m_wallCheckSize;
	[SerializeField, FoldoutGroup("Wall Check")] private Vector2 m_wallCheckOffset;
	[FoldoutGroup("Events")] public UnityEvent OnGroundEvent;

	private void Awake()
	{
		Rigidbody = GetComponent<Rigidbody2D>();
		m_lastScale = transform.localScale;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void Update()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.

		if (m_GroundCheck == null)
			return;
		Colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < Colliders.Length; i++)
		{
			if (Colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				OnGroundEvent.Invoke();
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}


	public void Move(float move, bool crouch, bool jump)
	{
		if (!CanMove)
			return;

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
			if (crouch)
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

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f * m_SpeedMultiplier, Rigidbody.velocity.y);

			if (CollidedWithWall() && ((m_FacingRight && move > 0) || (!m_FacingRight && move < 0)))
            {
				targetVelocity = new Vector3(0f, Rigidbody.velocity.y);
            }

			// And then smoothing it out and applying it to the character
			Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			Jump();
		}
	}

	private bool CollidedWithWall()
	{
		bool walls = Physics2D.Linecast(CalculateWallCheckStartPosition(), CalculateWallCheckEndPosition(), m_wallLayer);

		return walls;
	}

	#region  Public Methods
	public void Jump()
	{
		Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0f);
		Rigidbody.AddForce(new Vector2(0f, m_JumpForce));
	}

	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = m_lastScale;
		theScale.x *= -1;

		m_lastScale = theScale;

		transform.DOScale(theScale, .2f);
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

	private Vector2 CalculateWallCheckStartPosition() => (Vector2)transform.position + m_wallCheckOffset;

	private Vector2 CalculateWallCheckEndPosition()
    {
		return new Vector2(CalculateWallCheckStartPosition().x + m_wallCheckSize * Mathf.Sign(transform.localScale.x), CalculateWallCheckStartPosition().y);
    }
}