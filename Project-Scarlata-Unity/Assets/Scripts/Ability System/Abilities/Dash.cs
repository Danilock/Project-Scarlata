using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Rewriters.Player;

namespace Rewriters.AbilitySystem
{
    [CreateAssetMenu(fileName = "Dash", menuName = "Abilities/Dash")]
    public class Dash : BaseAbility
    {
        [SerializeField, FoldoutGroup("Force")] protected float Force;
        [SerializeField, FoldoutGroup("Force")] protected ForceMode2D ForceMode;
        [SerializeField] protected float Duration;
        [SerializeField] private float _dashReduction = 2f;
        public override void Activate(AbilityHolder holder)
        {
            //Preveting the character controller movement while dashing.
            holder.Owner.GetComponent<CharacterController2D>().CanMove = false;

            //Tells the animator set the dash.
            holder.Owner.Animator.SetBool("Dash", true);

            //Start dash process.
            ApplyDashToCharacter(holder);
        }

        protected void ApplyDashToCharacter(AbilityHolder holder){
            //Starging a coroutine that will stop the character for few seconds.
            holder.Owner.StartCoroutine(StopCharacterGravityForFewSeconds(holder, Duration));
            
            //Adding dash force.
            holder.Owner.Rigidbody.AddForce(CalculateDirectionOfDash(holder), 0f);
        }
        /// <summary>
        /// Stops Y gravity of this character for few seconds.
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        protected IEnumerator StopCharacterGravityForFewSeconds(AbilityHolder holder, float seconds){
            
            float previousGravityScale = holder.Owner.Rigidbody.gravityScale;
            CharacterController2D ch2D = holder.Owner.GetComponent<CharacterController2D>();

            holder.Owner.Rigidbody.gravityScale = 0f;

            //Stoping character's velocity.
            holder.Owner.Rigidbody.velocity = Vector2.zero;

            yield return new WaitForSeconds(Duration);

            // If the character isn't wall climbing we set the Gravity as how it should be.
            // Note: In this condition, we should add all states that will change RGB's Gravity.
            if(holder.Owner.CurrentCharacterState != CharacterStates.WallClimbing)
                holder.Owner.Rigidbody.gravityScale = previousGravityScale;
            
            //Allowing the character to move again
            ch2D.CanMove = true;

            //Set animation to false/
            holder.Owner.Animator.SetBool("Dash", false);

            holder.CheckIfCharacterIsGroundedOnceAbilityFinishes();

            //We stop the character in the air to prevent continous forces.
            ch2D.Rigidbody.velocity = Vector2.zero;
        }

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

            endDirection.x = (move.x * Force) / _dashReduction;
            endDirection.y = (move.y * Force) / _dashReduction;

            return endDirection;
        }
    }
}