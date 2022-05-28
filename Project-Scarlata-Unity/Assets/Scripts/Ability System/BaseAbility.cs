using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Rewriters.AbilitySystem
{
    /// <summary>
    /// Base class for scriptable abilities.
    /// </summary>
    public abstract class BaseAbility : ScriptableObject
    {
        [FoldoutGroup("Cooldown And Casting Time")]public bool HasCooldown = true;
        [FoldoutGroup("Cooldown And Casting Time")] public float Cooldown = 1f;
        [FoldoutGroup("Cooldown And Casting Time")] public float CastingTime = 0f;

        [FoldoutGroup("Allowed States")] public List<CharacterStates> AllowedCharacterStates = new List<CharacterStates>() { CharacterStates.Idle };

        public abstract void Activate(AbilityHolder holder);
    }
}