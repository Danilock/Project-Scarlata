using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;

namespace Rewriters.HealthSystem.Attacks
{
    /// <summary>
    /// Base class for attacks.
    /// </summary>
    public class Attack : MonoBehaviour
    {
        #region Attack Settings
        [FormerlySerializedAs("Point")]
        [Header("Attack Settings")] 
        
        [SerializeField] private Transform _point;

        protected Transform Point => _point == null ? transform : _point;
        public bool IgnoreTargetInvulnerability = false;

        [SerializeField] protected float SecondsToReproduceAttack;

        protected Coroutine HandleAttackCoroutine;

        [SerializeField, Header("Animations")] protected bool HasAnimation;
        [SerializeField, ShowIf("HasAnimation")] protected Animator Animator;
        protected readonly int Hash_Attach = Animator.StringToHash("Attack");
        #endregion
        
        #region Damage Settings
        [Header("Damage Settings")]
        [SerializeField] protected float DamageAmount;
        [SerializeField] protected LayerMask Layers;
        #endregion

        public UnityEvent OnHit;
        
        /// <summary>
        /// Damageable owning this Attack
        /// </summary>
        public HealthComponent Owner;

        public virtual void StartAttackProcess() => HandleAttackCoroutine = StartCoroutine(HandleAttack_CO());

        protected virtual void HandleAttack()
        {

        }

        protected virtual IEnumerator HandleAttack_CO()
        {
            if(HasAnimation)
                Animator.SetTrigger(Hash_Attach);

            yield return new WaitForSeconds(SecondsToReproduceAttack);

            HandleAttack();
        }

        protected virtual void Awake()
        {
            if (Owner == null)
                Owner = GetComponent<HealthComponent>();
        }

        public void SetDamage(float amount) => DamageAmount = amount;
    }
}