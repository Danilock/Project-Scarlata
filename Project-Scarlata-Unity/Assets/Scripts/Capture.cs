using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Capture : MonoBehaviour
{
    [SerializeField] private bool _canTakeScreenshot = true;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private float _captureTimeOnScreen;

    [SerializeField] private Image _image;
    [SerializeField] private Image _whiteImageEffect;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U) && _canTakeScreenshot)
        {
            StartCoroutine(TakeScreenshot_CO());
        }
    }

    private IEnumerator TakeScreenshot_CO()
    {
        _canTakeScreenshot = false;

        _image.sprite = null;

        _image.SetImageAlpha(0f);

        yield return new WaitForEndOfFrame();

        var screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        var sprite = Sprite.Create(screenshot, new Rect(0.0f, 0.0f, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f), 100.0f);

        _whiteImageEffect.SetImageAlpha(1f);

        _image.sprite = sprite;

        _image.SetImageAlpha(1f);

        _image.DOFade(0f, _captureTimeOnScreen).OnComplete(() => _image.sprite = null);

        _whiteImageEffect.DOFade(0f, _captureTimeOnScreen / 2f);

        yield return new WaitForSeconds(_cooldown);

        _canTakeScreenshot = true;
    }
}


public static class ImageExtensions
{
    public static void SetImageAlpha(this Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;

        img.color = color;
    }
}
