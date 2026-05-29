using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeInDuration = 1f;
    [SerializeField] private float _fadeOutDuration = 1f;

    public IEnumerator FadeInBlack()
    {
        yield return FadeTo(1f, _fadeInDuration);
    }


    public IEnumerator FadeOutBlack()
    {
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
