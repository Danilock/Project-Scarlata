using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.Player
{
    public class PlayerInput : PersistentSingleton<PlayerInput>
    {
        public Vector2 Move;

        public bool Jump;

        public bool Dash;

        public bool Transform;

        public bool Attack;

        private void Update()
        {
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Jump = Input.GetButtonDown("Jump");

            Dash = Input.GetButtonDown("Dash");
            
            Attack = Input.GetButtonDown("Attack");

            Transform = Input.GetButtonDown("Transform");
        }
    }
}