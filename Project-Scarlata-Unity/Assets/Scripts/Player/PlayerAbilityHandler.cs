using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;

namespace Rewriters.Player
{
    public class PlayerAbilityHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput _input;

        [SerializeField] private AbilityHolder _dashHolder;

        [SerializeField] private List<AbilityHolder> _playerAbilities;

        private void Awake()
        {
            _input = GetComponent<PlayerInput>();
        }

        // Update is called once per frame
        void Update()
        {
            if (_input.Dash)
            {
                _dashHolder.TriggerAbility();
            }
        }

        /// <summary>
        /// Sets an state of the given ability.
        /// </summary>
        /// <param name="abilty">Ability to change.</param>
        /// <param name="state">State to set to this ability.</param>
        public void SetPlayerAbilityState(BaseAbility abilty, AbilityStates state)
        {
            AbilityHolder holder = _playerAbilities.Find(x => x.Ability == abilty);

            if (holder == null)
                return;

            holder.SetAbilityState(state);
        }
    }
}