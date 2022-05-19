using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Player
{
    public class PlayerInput : MonoBehaviour
    {
        public float HorizontalAxis;

        public bool JumpWasPressedThisFrame;

        private void Update()
        {
            HorizontalAxis = Input.GetAxisRaw("Horizontal");

            JumpWasPressedThisFrame = Input.GetButtonDown("Jump");
        }
    }
}