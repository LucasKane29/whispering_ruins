using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CreditsScroller : MonoBehaviour
{
    [SerializeField] private RectTransform _creditsContainer;
    [SerializeField] private float _scrollSpeed = 50f;
    [SerializeField] private float _delayBeforeStart = 1f;
    [SerializeField] private UnityEvent _onCreditsFinished;

    private void Start() => StartCoroutine(ScrollCredits());

    private IEnumerator ScrollCredits()
    {
        yield return new WaitForSeconds(_delayBeforeStart);

        float endY = _creditsContainer.rect.height + Screen.height * 0.5f;

        while (_creditsContainer.anchoredPosition.y < endY)
        {
            _creditsContainer.anchoredPosition += Vector2.up * _scrollSpeed * Time.deltaTime;
            yield return null;
        }

        _onCreditsFinished?.Invoke();
    }
}
