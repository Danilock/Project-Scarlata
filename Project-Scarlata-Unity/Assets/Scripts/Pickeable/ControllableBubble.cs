using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;

namespace Rewriters.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ControllableBubble : Pickeable
    {
        [SerializeField, ReadOnly] private Vector2 _initialPosition;
        [SerializeField] private ControllableBubbleStates _currentState = ControllableBubbleStates.Idle;
        [SerializeField] private float _force = 3f;
        [SerializeField] private ForceMode2D _forceMode;

        protected override void Awake()
        {
            base.Awake();

            _initialPosition = transform.position;
        }

        private void Update()
        {

        }

        private void FixedUpdate()
        {
            if (_currentState != ControllableBubbleStates.OnUse)
                return;

            Rigidbody.AddForce(new Vector2(
                PlayerInput.Instance.Move.x * _force * Time.fixedDeltaTime, 
                PlayerInput.Instance.Move.y * _force * Time.fixedDeltaTime), 
                _forceMode
                );
        }

        protected override void OnPick(GameObject owner)
        {
            CharacterController2D ch2D = owner.GetComponent<CharacterController2D>();
            ch2D.enabled = false;

            Rigidbody2D playerRGB = owner.GetComponent<Rigidbody2D>();
            playerRGB.bodyType = RigidbodyType2D.Kinematic;
            playerRGB.velocity = Vector2.zero;

            Collider2D playerCollider = owner.GetComponent<Collider2D>();
            playerCollider.isTrigger = true;

            PlayerMovement playerMovement = owner.GetComponent<PlayerMovement>();
            playerMovement.enabled = false;

            owner.transform.SetParent(this.transform);
            owner.transform.position = transform.position;

            _currentState = ControllableBubbleStates.OnUse;
        }
    }

    public enum ControllableBubbleStates
    {
        OnUse,
        Returning,
        Idle
    }
}