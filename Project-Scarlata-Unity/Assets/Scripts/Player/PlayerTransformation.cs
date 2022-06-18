using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;

namespace Rewriters.Player
{
    [CreateAssetMenu(fileName = "Player Transformation", menuName = "Abilities/Player/Transformation")]
    public class PlayerTransformation : BaseAbility
    {
        private readonly int Hash_DarkMode = Animator.StringToHash("DarkMode");
        private readonly int Hash_Transform = Animator.StringToHash("Transform");

        public override void Activate(AbilityHolder holder)
        {
            PlayerAbilityHandler handler = holder.GetComponent<PlayerAbilityHandler>();

            holder.Owner.Animator.SetTrigger(Hash_Transform);
            holder.Owner.Animator.SetFloat(Hash_DarkMode, (float) handler.TransformationMode);
        }
    }
}