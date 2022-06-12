using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AbilitySystem
{
    [CreateAssetMenu(fileName = "Transformation", menuName = "Abilities/Player Transformation")]
    public class PlayerTransformation : BaseAbility
    {
        private readonly int Hash_Transform = Animator.StringToHash("Transform");

        public override void Activate(AbilityHolder holder)
        {
            holder.Owner.Animator.SetTrigger(Hash_Transform);
        }
    }
}