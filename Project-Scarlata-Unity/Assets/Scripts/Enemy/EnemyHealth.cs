using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.HealthSystem;
using Sirenix.OdinInspector;

namespace Rewriters.Enemies
{
    public class EnemyHealth : HealthComponent
    {
        [SerializeField] protected bool StunOnGettingDamage;
        [SerializeField, ShowIf("StunOnGettingDamage")] protected float SecondsStunned = 2f;
        [SerializeField] EnemyAttack Attack;
        protected EnemyController EnemyController;
        public Animator Animator;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
        }

        protected override void Start()
        {
            base.Start();

            EnemyController = GetComponent<EnemyController>();
        }

        protected override void OnTakeDamage(DamageInfo incomingDamage)
        {
            EnemyController.AIAgent.StopAgent(SecondsStunned);
            Attack.DisallowAttackForFewSeconds(SecondsStunned);

            Animator.SetTrigger("TakeDamage");
        }

        public override void OnDeath()
        {
            base.OnDeath();

            EnemyController.AIAgent.ForceStopAgent();

            Animator.SetBool("Death", true);
        }
    }
}