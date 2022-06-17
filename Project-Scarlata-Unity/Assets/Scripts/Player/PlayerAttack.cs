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
        private readonly int Hash_Ability = Animator.StringToHash("Ability");
        private readonly int Hash_Index = Animator.StringToHash("AttackIndex");
        private readonly int Hash_Attack = Animator.StringToHash("Attack");

        public float Damage;

        public override void Activate(AbilityHolder holder)
        {
            holder.Owner.Animator.SetTrigger(Hash_Ability);
            holder.Owner.Animator.SetTrigger(Hash_Attack);
            holder.Owner.Animator.SetFloat(Hash_Index, holder.CurrentSequence);
        }
    }
}