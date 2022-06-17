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

        public int CurrentSequence;

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
            if (Ability.RequiresMana && ManaSource.GetAmount < Ability.ManaRequired)
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

            yield return new WaitForSeconds(Ability.CastingTime);

            Ability.Activate(this);

            CurrentAbilityState = AbilityStates.Cooldown;

            Owner.Animator.SetTrigger(Hash_Ability);

            OnTriggerAbility?.Invoke();

            HandleSequence();

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
            CurrentAbilityState = newState;
        }
        
        public virtual void SetAbilityState(int newState){
            CurrentAbilityState = (AbilityStates) newState;
        }

        protected virtual void HandleSequence()
        {
            if (!Ability.HasSequence)
                return;

            CurrentSequence = (CurrentSequence + 1) % Ability.AmountOfSequences;
        }
    }

    public enum AbilityStates
    {
        ReadyToUse = 0,
        Casting = 1,
        Cooldown = 2
    }
}