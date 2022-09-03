using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

namespace Rewriters
{
    public class UIManager : PersistentSingleton<UIManager>
    {
        [SerializeField] private Image _screenFadeImage;
        [SerializeField] private Capture _capture;

        /// <summary>
        /// Does a screen fade.
        /// </summary>
        /// <param name="color">Fade's color.</param>
        /// <param name="time">Fades's time.</param>
        public void DoScreenFade(Color color, float to, float time, Action action = null, bool jumpBackToInitialAlpha = false)
        {
            float initialAlpha = _screenFadeImage.color.a;

            _screenFadeImage.color = color;
            _screenFadeImage.SetImageAlpha(1f);
            _screenFadeImage.DOFade(to, time).OnComplete(() =>
            {
                action?.Invoke();
            });

            if (jumpBackToInitialAlpha)
            {
                DoScreenFade(color, initialAlpha, time, null, false);
            }
        }
        
        /// <summary>
        /// Does a screen fade. Specify the fade's image initial alpha value.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="time"></param>
        /// <param name="initialFadeAlpha"></param>
        public void DoScreenFade(Color color, float to, float time, float initialFadeAlpha, Action action = null, bool jumpBackToInitialAlpha = false)
        {
            float initialAlpha = _screenFadeImage.color.a;

            _screenFadeImage.color = color;
            _screenFadeImage.SetImageAlpha(initialFadeAlpha);
            _screenFadeImage.DOFade(to, time).OnComplete(() =>
            {
                action?.Invoke();
            });

            if (jumpBackToInitialAlpha)
            {
                DoScreenFade(color, initialAlpha, time, null, false);
            }
        }

        public void Screenshot()
        {
            _capture.TakeScreenshot();
        }

        public void CustomScreenshot(float captureTimeOnScreen)
        {
            _capture.TakeCustomScreenshot(captureTimeOnScreen);
        }
    }
}