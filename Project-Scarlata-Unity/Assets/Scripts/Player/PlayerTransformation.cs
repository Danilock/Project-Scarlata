using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewriters.AbilitySystem;
using Sirenix.OdinInspector;

namespace Rewriters.Player
{
    [CreateAssetMenu(fileName = "Player Transformation", menuName = "Abilities/Player/Transformation")]
    public class PlayerTransformation : BaseAbility
    {
        [SerializeField, FoldoutGroup("Scren Fade Settings")] private float _castTimeToTriggerScreenFade;
        [SerializeField, FoldoutGroup("Scren Fade Settings")] private float _fadeTime;
        [SerializeField, FoldoutGroup("Scren Fade Settings")] private Color _lightModeFadeColor;
        [SerializeField, FoldoutGroup("Scren Fade Settings")] private Color _darkModeFadeColor;
        [SerializeField, FoldoutGroup("Scren Fade Settings")] private float _initialFadeAlpha;

        private readonly int Hash_DarkMode = Animator.StringToHash("DarkMode");
        private readonly int Hash_Transform = Animator.StringToHash("Transform");

        public override void Activate(AbilityHolder holder)
        {
            PlayerAbilityHandler handler = holder.GetComponent<PlayerAbilityHandler>();

            holder.Owner.Animator.SetTrigger(Hash_Transform);
            holder.Owner.Animator.SetFloat(Hash_DarkMode, (float) handler.TransformationMode);
            handler.TriggerTransformationVFXAnimation();

            holder.StartCoroutine(ScreenFade_CO(handler));
        }

        private IEnumerator ScreenFade_CO(PlayerAbilityHandler handler)
        {
            yield return new WaitForSeconds(_castTimeToTriggerScreenFade);

            UIManager.Instance.DoScreenFade(DetermineColorBasedOnPlayerTransformationMode(handler), _fadeTime, _initialFadeAlpha);
        }

        private Color DetermineColorBasedOnPlayerTransformationMode(PlayerAbilityHandler handler)
        {
            return handler.TransformationMode == PlayerTransformationMode.LightMode ? _darkModeFadeColor : _lightModeFadeColor;
        }
    }
}