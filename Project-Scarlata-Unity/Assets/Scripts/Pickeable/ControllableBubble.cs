using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;
using DG.Tweening;

namespace Rewriters.Items
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class ControllableBubble : Pickeable
    {
        [SerializeField, ReadOnly] private Vector2 _initialPosition;
        [SerializeField] private ControllableBubbleStates _currentState = ControllableBubbleStates.Idle;
        [SerializeField] private float _bubbleLifeTime = 2f;
        [SerializeField] private float _jumpForce = 25f;

        private Coroutine _stopBubbleAfterSeconds;

        protected override void Awake()
        {
            base.Awake();

            _initialPosition = transform.position;
        }

        protected override void OnPick(Character owner)
        {
            if (_currentState != ControllableBubbleStates.Idle)
                return;

            StopPlayerPhysics();

            Rigidbody.bodyType = RigidbodyType2D.Dynamic;

            _currentState = ControllableBubbleStates.OnUse;

            owner.GetComponent<Character>().SetCharacterState(CharacterStates.InBubbles);

            StartCoroutine(HandleBubbleBeforeStart_CO());
        }

        private void Update()
        {
            if (_currentState != ControllableBubbleStates.OnUse)
                return;

            if (PlayerInput.Instance.Jump)
            {
                if (_stopBubbleAfterSeconds != null)
                    StopCoroutine(_stopBubbleAfterSeconds);

                ReanudatePlayerPhysics(true);
            }
        }

        private IEnumerator HandleBubbleBeforeStart_CO()
        {
            yield return new WaitForSeconds(1f);
            Rigidbody.velocity = PlayerInput.Instance.Move * 10f;

            _stopBubbleAfterSeconds = StartCoroutine(StopBubbleAfterSeconds_CO());
        }

        private IEnumerator StopBubbleAfterSeconds_CO()
        {
            yield return new WaitForSeconds(_bubbleLifeTime);

            ReanudatePlayerPhysics(false);
        }

        private void StopPlayerPhysics()
        {
            CharacterController2D ch2D = Owner.GetComponent<CharacterController2D>();
            ch2D.enabled = false;

            Rigidbody2D playerRGB = Owner.GetComponent<Rigidbody2D>();
            playerRGB.bodyType = RigidbodyType2D.Kinematic;
            playerRGB.velocity = Vector2.zero;

            Collider2D playerCollider = Owner.GetComponent<Collider2D>();
            playerCollider.isTrigger = true;

            PlayerMovement playerMovement = Owner.GetComponent<PlayerMovement>();
            playerMovement.enabled = false;

            Owner.transform.SetParent(this.transform);
            Owner.transform.position = transform.position;
        }

        private void ResetBubble()
        {
            _currentState = ControllableBubbleStates.Returning;

            Rigidbody.bodyType = RigidbodyType2D.Static;

            transform.DOMove(_initialPosition, .5f).OnComplete(() =>
            {
            _currentState = ControllableBubbleStates.Idle;
        });
        }

        private void ReanudatePlayerPhysics(bool jump)
        {
            ResetBubble();

            CharacterController2D ch2D = Owner.GetComponent<CharacterController2D>();
            ch2D.enabled = true;

            Rigidbody2D playerRGB = Owner.GetComponent<Rigidbody2D>();
            playerRGB.bodyType = RigidbodyType2D.Dynamic;
            playerRGB.velocity = Vector2.zero;

            Collider2D playerCollider = Owner.GetComponent<Collider2D>();
            playerCollider.isTrigger = false;

            PlayerMovement playerMovement = Owner.GetComponent<PlayerMovement>();
            playerMovement.enabled = true;

            Owner.GetComponent<Character>().SetCharacterState(CharacterStates.Jumping);

            Owner.transform.SetParent(null);

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
                
                ReanudatePlayerPhysics(false);

                ControllableBubble ctrBubble = collision.gameObject.GetComponent<ControllableBubble>();

                if(ctrBubble != null)
                {
                    ctrBubble.Pick(Owner);

                    if (_stopBubbleAfterSeconds != null)
                        StopCoroutine(_stopBubbleAfterSeconds);

                    ResetBubble();
                }
            }

            base.OnCollisionEnter2D(collision);
        }
    }

    public enum ControllableBubbleStates
    {
        OnUse,
        Returning,
        Idle
    }
}