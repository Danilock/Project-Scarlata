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
        [FoldoutGroup("Cooldown And Casting Time")] public bool HasCooldown = true;
        [FoldoutGroup("Cooldown And Casting Time"), ShowIf("HasCooldown")] public float Cooldown = 1f;
        [FoldoutGroup("Cooldown And Casting Time")] public bool HasCastingTime = false;
        [FoldoutGroup("Cooldown And Casting Time"), ShowIf("HasCastingTime")] public float CastingTime = 0f;

        [FoldoutGroup("Mana")]
        public bool RequiresMana = false;

        [FoldoutGroup("Mana"), ShowIf("@this.RequiresMana")]
        public float ManaRequired = 0f;

        [FoldoutGroup("Allowed States")] public List<CharacterStates> AllowedCharacterStates = new List<CharacterStates>() { CharacterStates.Idle };

        public virtual void OnAbilityUpdate(AbilityHolder holder) { }

        public abstract void Activate(AbilityHolder holder);
    }
}