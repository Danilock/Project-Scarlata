using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;
using Sirenix.OdinInspector;
using Rewriters.HealthSystem.Attacks;

namespace Rewriters.Player
{
    [CreateAssetMenu(menuName = "Abilities/Player/Attack", fileName = "Attack")]
    public class PlayerAttack : BaseAbility
    {
        private readonly int Hash_Index = Animator.StringToHash("AttackIndex");
        private readonly int Hash_Attack = Animator.StringToHash("Attack");

        [FoldoutGroup("Attacks Indexs")]
        [SerializeField] private int _currentAttackIndex = 0;
        [SerializeField] private int _maxAmountOfAttacks = 3;

        public float Damage;

        public override void Activate(AbilityHolder holder)
        {
            holder.Owner.Animator.ResetTrigger(Hash_Attack);

            holder.Owner.Animator.SetFloat(Hash_Index, _currentAttackIndex);
            holder.Owner.Animator.SetTrigger(Hash_Attack);

            MeleeAttack meleeAttack = holder.GetComponent<MeleeAttack>();
            meleeAttack.DoAttack();

            _currentAttackIndex = (_currentAttackIndex + 1) % (_maxAmountOfAttacks);
            holder.Owner.Animator.SetFloat(Hash_Index, _currentAttackIndex);
        }
    }
}