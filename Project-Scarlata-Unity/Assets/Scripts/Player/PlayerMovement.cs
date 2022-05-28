using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;

namespace Rewriters.Player
{
    [RequireComponent(typeof(CharacterController2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private CharacterController2D _ch2D;
        [SerializeField] private PlayerInput _input;

        private void Update()
        {
            _ch2D.Move(_input.Move.x, false, _input.Jump, false);
        }
    }
}