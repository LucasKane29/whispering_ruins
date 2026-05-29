using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ether : MonoBehaviour
{
    [SerializeField] private IntEventChannel _soulCountChannel;
    [SerializeField] private TMP_Text _count;
    [SerializeField] private float _animationDuration = 0.5f;

    private int _displayedValue = 0;
    private int _targetValue = 0;
    private Coroutine _animationCoroutine;

    void Awake()
    {
        _soulCountChannel.OnEventRaised += ApplyCount;
    }

    void Start()
    {
        _targetValue = _displayedValue = GameManager.Instance.Souls;
        if (_count != null)
            _count.text = _targetValue.ToString();
    }
    private void OnDestroy()
    {
        _soulCountChannel.OnEventRaised -= ApplyCount;
    }

    private void ApplyCount(int amount)
    {
        _targetValue += amount;
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }
        _animationCoroutine = StartCoroutine(AnimateCount(_displayedValue, _targetValue));
    }

    private IEnumerator AnimateCount(int from, int to)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _animationDuration)
        {
            elapsedTime += Time.deltaTime;
            _displayedValue = Mathf.RoundToInt(Mathf.Lerp(from, to, elapsedTime / _animationDuration));
            if (_count != null)
            {
                _count.text = _displayedValue.ToString();
            }
            yield return null;
        }
        _displayedValue = to;

        if (_count != null)
        {
            _count.text = to.ToString();
        }
        _animationCoroutine = null;
    }
}
