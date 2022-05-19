using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AbilitySystem
{
    public class AbilityHolder : MonoBehaviour
    {
        public Character Owner;
        public BaseAbility Ability;

        public AbilityStates CurrentAbilityState = AbilityStates.ReadyToUse;

        private IEnumerator _handleAbilityUsage;

        public void TriggerAbility()
        {
            if (CurrentAbilityState != AbilityStates.ReadyToUse)
                return;

            if (!CharacterIsOnAllowedState())
                return;

            _handleAbilityUsage = HandleAbilityUsage_CO();

            StartCoroutine(_handleAbilityUsage);
        }

        private IEnumerator HandleAbilityUsage_CO()
        {
            CurrentAbilityState = AbilityStates.Casting;

            yield return new WaitForSeconds(Ability.CastingTime);

            Ability.Activate(this);
            CurrentAbilityState = AbilityStates.Cooldown;

            yield return new WaitForSeconds(Ability.Cooldown);

            CurrentAbilityState = AbilityStates.ReadyToUse;
        }

        public bool CharacterIsOnAllowedState()
        {
            return Ability.AllowedCharacterStates.Contains(Owner.CurrentCharacterState);
        }
    }

    public enum AbilityStates
    {
        ReadyToUse,
        Casting,
        Cooldown
    }
}