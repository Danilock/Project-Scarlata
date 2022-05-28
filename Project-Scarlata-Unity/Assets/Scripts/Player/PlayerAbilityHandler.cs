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
    }
}