using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private CharacterStates _currentCharacterState = CharacterStates.Idle;
        public CharacterStates CurrentCharacterState
        {
            get => _currentCharacterState;
            private set => _currentCharacterState = value;
        }
        public void SetCharacterState(CharacterStates newState) => CurrentCharacterState = newState;
    }
}