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

        private Coroutine _handleAbilityUsage;

        private static readonly int Hash_Ability = Animator.StringToHash("Ability");

        public UnityEvent OnTriggerAbility;

        public ManaSource ManaSource;

        protected virtual void Awake()
        {
            if (Owner == null)
                Owner = GetComponent<Character>();
        }

        protected void Start()
        {
            ManaSource = GetComponent<ManaSource>();
        }

        [ContextMenu("Trigger Ability")]
        public virtual void TriggerAbility()
        {
            if (Ability.RequiresMana && ManaSource.GetCurrentAmount < Ability.ManaRequired)
                return;

            if (CurrentAbilityState != AbilityStates.ReadyToUse)
                return;

            if (!CharacterIsOnAllowedState())
                return;

            _handleAbilityUsage = StartCoroutine(HandleAbilityUsage_CO());
        }

        protected virtual void Update()
        {
           Ability.OnAbilityUpdate(this);
        }

        protected virtual IEnumerator HandleAbilityUsage_CO()
        {
            CurrentAbilityState = AbilityStates.Casting;

            if(Ability.HasCastingTime)
                yield return new WaitForSeconds(Ability.CastingTime);

            CurrentAbilityState = AbilityStates.Cooldown;

            Ability.Activate(this);

            Owner.Animator.SetTrigger(Hash_Ability);

            OnTriggerAbility?.Invoke();

            if(Ability.HasCooldown){
                StartCoroutine(HandleCooldown_CO());
            }
        }

        protected virtual IEnumerator HandleCooldown_CO(){
            yield return new WaitForSeconds(Ability.Cooldown);

            CurrentAbilityState = AbilityStates.ReadyToUse;
        }

        public virtual bool CharacterIsOnAllowedState()
        {
            return Ability.AllowedCharacterStates.Contains(Owner.CurrentCharacterState);
        }

        public virtual void SetAbilityState(AbilityStates newState){
            //Preventing the ability to be set to ready when it's LOCKED.
            if (newState == AbilityStates.ReadyToUse && CurrentAbilityState == AbilityStates.Locked)
                return;

            CurrentAbilityState = newState;
        }
        
        public virtual void SetAbilityState(int newState){
            //Preventing the ability to be set to ready when it's LOCKED.
            if (newState == (int)AbilityStates.ReadyToUse && CurrentAbilityState == AbilityStates.Locked)
                return;

            CurrentAbilityState = (AbilityStates) newState;
        }

        public virtual void UnlockAbility()
        {
            CurrentAbilityState = AbilityStates.ReadyToUse;
        }
    }

    public enum AbilityStates
    {
        ReadyToUse = 0,
        Casting = 1,
        Cooldown = 2,
        Locked = 3
    }
}