using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Rewriters
{
    public class Character : MonoBehaviour
    {
        [SerializeField, ReadOnly] private CharacterStates _currentCharacterState = CharacterStates.Idle;
        
        public Rigidbody2D Rigidbody;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();   
        }

        public CharacterStates CurrentCharacterState
        {
            get => _currentCharacterState;
            private set => _currentCharacterState = value;
        }
        public void SetCharacterState(CharacterStates newState) => CurrentCharacterState = newState;
    }
}