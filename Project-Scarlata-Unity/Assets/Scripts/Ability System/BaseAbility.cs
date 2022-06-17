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

        [FoldoutGroup("Mana")]
        public bool RequiresMana = false;

        [FoldoutGroup("Mana"), ShowIf("@this.RequiresMana")]
        public float ManaRequired = 0f;

        [FoldoutGroup("Allowed States")] public List<CharacterStates> AllowedCharacterStates = new List<CharacterStates>() { CharacterStates.Idle };

        /// <summary>
        /// Sequences are used to determine if this ability has different sequences such as an attack.
        /// </summary>
        [FoldoutGroup("Sequence")] public bool HasSequence = false;
        [FoldoutGroup("Sequence"), ShowIf("@this.HasSequence")] public int AmountOfSequences;

        public virtual void OnAbilityUpdate(AbilityHolder holder) { }

        public abstract void Activate(AbilityHolder holder);
    }
}