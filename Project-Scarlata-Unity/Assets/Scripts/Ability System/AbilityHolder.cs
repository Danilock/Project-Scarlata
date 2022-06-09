using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Rewriters.AbilitySystem
{
    public class AbilityHolder : MonoBehaviour
    {
        public Character Owner;
        public BaseAbility Ability;

        public AbilityStates CurrentAbilityState = AbilityStates.ReadyToUse;

        private IEnumerator _handleAbilityUsage;

        private static readonly int Hash_Ability = Animator.StringToHash("Ability");

        public UnityEvent OnTriggerAbility;


        [ContextMenu("Trigger Ability")]
        public void TriggerAbility()
        {
            if (CurrentAbilityState != AbilityStates.ReadyToUse)
                return;

            if (!CharacterIsOnAllowedState())
                return;

            _handleAbilityUsage = HandleAbilityUsage_CO();

            StartCoroutine(_handleAbilityUsage);
        }

        private void Update()
        {
           Ability.OnAbilityUpdate(this);
        }

        private IEnumerator HandleAbilityUsage_CO()
        {
            CurrentAbilityState = AbilityStates.Casting;

            yield return new WaitForSeconds(Ability.CastingTime);

            Ability.Activate(this);
            CurrentAbilityState = AbilityStates.Cooldown;
            Owner.Animator.SetTrigger(Hash_Ability);
            OnTriggerAbility?.Invoke();

            if(Ability.HasCooldown){
                StartCoroutine(HandleCooldown_CO());
            }
        }

        private IEnumerator HandleCooldown_CO(){
            yield return new WaitForSeconds(Ability.Cooldown);

            CurrentAbilityState = AbilityStates.ReadyToUse;
        }

        public bool CharacterIsOnAllowedState()
        {
            return Ability.AllowedCharacterStates.Contains(Owner.CurrentCharacterState);
        }

        public void SetAbilityState(AbilityStates newState){
            CurrentAbilityState = newState;
        }
        
        public void SetAbilityState(int newState){
            CurrentAbilityState = (AbilityStates) newState;
        }
    }

    public enum AbilityStates
    {
        ReadyToUse = 0,
        Casting = 1,
        Cooldown = 2
    }
}