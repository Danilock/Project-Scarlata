using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rewriters.AbilitySystem
{
    /// <summary>
    /// Base class for scriptable abilities.
    /// </summary>
    public abstract class BaseAbility : ScriptableObject
    {
        public float Cooldown = 1f;
        public float CastingTime = 0f;

        public List<CharacterStates> AllowedCharacterStates = new List<CharacterStates>() { CharacterStates.Idle };

        public abstract void Activate(AbilityHolder holder);
    }
}