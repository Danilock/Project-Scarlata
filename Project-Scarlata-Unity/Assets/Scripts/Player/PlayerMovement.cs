using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _ch2D.Move(_input.HorizontalAxis, false, _input.JumpWasPressedThisFrame);
        }
    }
}