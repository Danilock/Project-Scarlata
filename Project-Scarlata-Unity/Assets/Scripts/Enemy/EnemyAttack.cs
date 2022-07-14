using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AI;
using Rewriters.HealthSystem.Attacks;
using Sirenix.OdinInspector;
using Rewriters.HealthSystem;

namespace Rewriters.Enemies
{
    [RequireComponent(typeof(MeleeAttack))]
    [RequireComponent(typeof(TargetDetection))]
    public class EnemyAttack : MonoBehaviour 
    {
        [SerializeField, FoldoutGroup("Dependencies")] protected Attack Attack;
        [SerializeField, FoldoutGroup("Dependencies")] protected TargetDetection TargetDetection;
        [SerializeField, FoldoutGroup("Accessibility")] protected float Cooldown;
        [SerializeField, FoldoutGroup("Accessibility")] protected bool CanAttack = true;
        [SerializeField, FoldoutGroup("Accessibility")] protected bool AttackOnlyWhenGrounded = true;
        [SerializeField, FoldoutGroup("AI")] protected AIAgent Agent;
        [SerializeField, FoldoutGroup("AI")] protected bool ShouldStopWhileAttacking;
        [SerializeField, FoldoutGroup("AI"), ShowIf("ShouldStopWhileAttacking")] protected float SecondsStopped = 3f;

        [SerializeField] protected bool InvulnerableWhileDoingAttack = false;
        [SerializeField, ShowIf("InvulnerableWhileDoingAttack")] protected float InvulnerabilityTime;
        [SerializeField, ShowIf("InvulnerableWhileDoingAttack")] protected HealthComponent HealthComponent;

        #region Coroutines Fields
        protected Coroutine DisallowAttack;
        #endregion

        private void Update()
        {
            if (AttackOnlyWhenGrounded && !Agent.IsGrounded())
                return;

            if(TargetDetection.IsDetectingATarget() && CanAttack)
            {
                DoAttack();
            }
        }

        public virtual void DoAttack()
        {
            Attack.StartAttackProcess();
            CheckIfShouldBeInvulnerable();
            CheckAIStop();

            HandleCooldown();
        }

        protected virtual void HandleCooldown()
        {
            StartCoroutine(DisallorAttackForFewSeconds_CO(SecondsStopped));
        }

        protected virtual IEnumerator DisallorAttackForFewSeconds_CO(float seconds)
        {
            CanAttack = false;

            yield return new WaitForSeconds(Cooldown);

            CanAttack = true;
        }

        protected virtual void CheckIfShouldBeInvulnerable()
        {
            if (!InvulnerableWhileDoingAttack)
                return;

            HealthComponent.SetInvulnerableForSeconds(InvulnerabilityTime);
        }

        protected virtual void CheckAIStop()
        {
            if (!ShouldStopWhileAttacking)
                return;

            Agent.StopAgent(SecondsStopped);
        }

        public virtual void DisallowAttackForFewSeconds(float seconds)
        {
            if (DisallowAttack != null)
                StopCoroutine(DisallowAttack);

            DisallowAttack = StartCoroutine(DisallorAttackForFewSeconds_CO(seconds));
        }
    }
}