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
        public Animator Animator;

        public CharacterController2D Ch2D;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody2D>();
            Animator = GetComponent<Animator>();
            Ch2D = GetComponent<CharacterController2D>();
        }

        public CharacterStates CurrentCharacterState
        {
            get => _currentCharacterState;
            private set => _currentCharacterState = value;
        }

        private void Update()
        {
            Debug.Log(CurrentCharacterState);
        }
        public void SetCharacterState(CharacterStates newState) => CurrentCharacterState = newState;
    }
}