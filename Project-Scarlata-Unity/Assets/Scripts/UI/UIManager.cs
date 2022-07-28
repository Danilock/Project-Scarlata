using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Rewriters
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private Image _screenFadeImage;
        /// <summary>
        /// Does a screen fade.
        /// </summary>
        /// <param name="color">Fade's color.</param>
        /// <param name="time">Fades's time.</param>
        public void DoScreenFade(Color color, float time)
        {
            _screenFadeImage.color = color;
            _screenFadeImage.SetImageAlpha(1f);
            _screenFadeImage.DOFade(0f, time);
        }
        
        /// <summary>
        /// Does a screen fade. Specify the fade's image initial alpha value.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="time"></param>
        /// <param name="initialFadeAlpha"></param>
        public void DoScreenFade(Color color, float time, float initialFadeAlpha)
        {
            _screenFadeImage.color = color;
            _screenFadeImage.SetImageAlpha(initialFadeAlpha);
            _screenFadeImage.DOFade(0f, time);
        }
    }
}