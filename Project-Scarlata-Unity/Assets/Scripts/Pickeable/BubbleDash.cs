using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.Player;
using Rewriters.AbilitySystem;

namespace Rewriters.Items
{
    public class BubbleDash : Pickeable
    {
        [SerializeField] private BaseAbility _dashAbilityProfile;
        protected override void OnPick(GameObject owner)
        {
            PlayerAbilityHandler handler = owner.GetComponent<PlayerAbilityHandler>();

            if (handler == null)
            {
                return;
            }

            handler.SetPlayerAbilityState(_dashAbilityProfile, AbilityStates.ReadyToUse);
        }
    }
}