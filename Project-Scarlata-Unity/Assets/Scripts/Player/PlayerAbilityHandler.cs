using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;
using Sirenix.OdinInspector;

namespace Rewriters.Player
{
    public class PlayerAbilityHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        private List<AbilityHolder> _holders = new List<AbilityHolder>();

        [SerializeField] private List<BaseAbility> _abilities;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }

        private void Start()
        {
            GenerateHolders();
        }

        // Update is called once per frame
        void Update()
        {
            if (_input.Dash)
            {
                GetAbility<Dash>().TriggerAbility();
            }
        }

        /// <summary>
        /// Sets an state of the given ability.
        /// </summary>
        /// <param name="abilty">Ability to change.</param>
        /// <param name="state">State to set to this ability.</param>
        public void SetPlayerAbilityState(BaseAbility abilty, AbilityStates state)
        {
            AbilityHolder holder = _holders.Find(x => x.Ability == abilty);

            if (holder == null)
                return;

            holder.SetAbilityState(state);
        }
        
        public void SetDashState(int newState)
        {
            GetAbility<Dash>().SetAbilityState(newState);
        }

        private void GenerateHolders()
        {
            foreach(BaseAbility ability in _abilities)
            {
                AbilityHolder currentHolder = gameObject.AddComponent<AbilityHolder>();
                currentHolder.Ability = ability;

                _holders.Add(currentHolder);
            }
        }

        private AbilityHolder GetAbility<T>() where T : BaseAbility
        {
            return _holders.Find(x => x.Ability.GetType() == typeof(T));
        }
    }
}