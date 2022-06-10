using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;
using ObjectPooling;

namespace Rewriters.AbilitySystem
{
    [CreateAssetMenu(fileName = "Dash", menuName = "Abilities/Dash")]
    public class Dash : BaseAbility
    {
        [SerializeField, FoldoutGroup("Force")] protected float Force;
        [SerializeField, FoldoutGroup("Force")] protected ForceMode2D ForceMode;
        [SerializeField] protected float Duration;
        [SerializeField, Tooltip("Force reduced while doing a diagonal dash.")] protected float DashReduction = 1.5f;

        [SerializeField, FoldoutGroup("Fade")] protected string FadePoolName = "Player_Fade";
        [SerializeField, FoldoutGroup("Fade")] protected int AmountOfFades = 4;
        [SerializeField, FoldoutGroup("Fade")] protected float[] GeneralAlphaEffectAmount;

        private Coroutine _CharacterGravityCoroutine;

        public override void Activate(AbilityHolder holder)
        {
            //Preveting the character controller movement while dashing.
            holder.Owner.GetComponent<CharacterController2D>().CanMove = false;
            holder.Owner.GetComponent<CharacterController2D>().IsInAirDueToWallJump = false;

            //Tells the animator set the dash.
            holder.Owner.Animator.SetBool("Dash", true);

            //Start dash process.
            ApplyDashToCharacter(holder);
        }

        protected void ApplyDashToCharacter(AbilityHolder holder){
            if (_CharacterGravityCoroutine != null)
                holder.Owner.StopCoroutine(_CharacterGravityCoroutine);

            //Starging a coroutine that will stop the character for few seconds.
            _CharacterGravityCoroutine = holder.Owner.StartCoroutine(StopCharacterGravityForFewSeconds(holder, Duration));

            //InstantiateEffects
            holder.Owner.StartCoroutine(InstantiateFadeEffect(holder));

            //Adding dash force.
            holder.Owner.Rigidbody.AddForce(CalculateDirectionOfDash(holder), 0f);
        }

        private IEnumerator InstantiateFadeEffect(AbilityHolder holder)
        {
            int i = 0;

            while(i < AmountOfFades)
            {
                GameObject gm = ObjectPooler.Instance.GetObjectFromPool(FadePoolName);

                gm.transform.localScale = holder.transform.localScale;
                gm.transform.position = holder.transform.position;

                Material mat = gm.GetComponent<SpriteRenderer>().material;
                mat.SetFloat("_Alpha", GeneralAlphaEffectAmount[i]);

                yield return new WaitForSeconds(Duration / AmountOfFades);

                i++;
            }
        }

        /// <summary>
        /// Stops Y gravity of this character for few seconds.
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        protected IEnumerator StopCharacterGravityForFewSeconds(AbilityHolder holder, float seconds){

            CharacterController2D ch2D = holder.Owner.GetComponent<CharacterController2D>();

            holder.Owner.Rigidbody.gravityScale = 0f;

            //Stoping character's velocity.
            holder.Owner.Rigidbody.velocity = Vector2.zero;

            yield return new WaitForSeconds(Duration);

            // If the character isn't wall climbing we set the Gravity as how it should be.
            // Note: In this condition, we should add all states that will change RGB's Gravity.
            if(holder.Owner.CurrentCharacterState != CharacterStates.WallClimbing)
                holder.Owner.Rigidbody.gravityScale = 5f;
            
            //Allowing the character to move again
            ch2D.CanMove = true;

            //Set animation to false/
            holder.Owner.Animator.SetBool("Dash", false);

            //We stop the character in the air to prevent continous forces.
            if (!ch2D.IsInAirDueToWallJump)
                ch2D.Rigidbody.velocity = Vector2.zero;

            yield return new WaitForSeconds(.5f);

            if (ch2D.IsGrounded)
                holder.SetAbilityState(AbilityStates.ReadyToUse);
        }

        /// <summary>
        /// Calculates the direction depending on player's horizontal input.
        /// </summary>
        /// <param name="holder"></param>
        /// <returns></returns>
        private Vector2 CalculateDirectionOfDash(AbilityHolder holder){
            Vector2 move = holder.GetComponent<PlayerInput>().Move;

            if(move.x == 0 && move.y == 0)
            {
                return new Vector2(Mathf.Sign(holder.transform.localScale.x) * Force, 0f);
            }

            Vector2 endDirection;
            
            if(move.x != 0f && move.y == 0f)
            {
                return new Vector2(move.x * Force, 0f);
            }

            if(move.y != 0f && move.x == 0f)
            {
                return new Vector2(0f, move.y * Force);
            }

            endDirection.x = (move.x * Force) / DashReduction;
            endDirection.y = (move.y * Force) / DashReduction;

            return endDirection;
        }
    }
}