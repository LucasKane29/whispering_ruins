using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeInDuration = 1f;
    [SerializeField] private float _fadeOutDuration = 1f;
    [SerializeField] private float _minimumDisplayTime = 5f;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Sprite[] _backgrounds;

    public float FadeInDuration => _fadeInDuration;
    private float _showStartTime;

    public IEnumerator FadeInBlack()
    {
        if (_backgroundImage != null && _backgrounds != null && _backgrounds.Length > 0)
            _backgroundImage.sprite = _backgrounds[Random.Range(0, _backgrounds.Length)];

        yield return FadeTo(1f, _fadeInDuration);
        _showStartTime = Time.unscaledTime;
    }

    public IEnumerator FadeOutBlack(bool skipMinimumDisplay = false)
    {
        if (!skipMinimumDisplay)
        {
            float elapsed = Time.unscaledTime - _showStartTime;
            float remaining = _minimumDisplayTime - elapsed;
            if (remaining > 0f)
                yield return new WaitForSecondsRealtime(remaining);
        }

        yield return FadeTo(0f, _fadeOutDuration);
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsedTime / duration));
            yield return null;
        }
        _canvasGroup.alpha = targetAlpha;
    }
}
