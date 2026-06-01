using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject _door;
    [SerializeField] private GameObject _light;
    [SerializeField] private EventChannel _eventChannel;

    [SerializeField] private AudioClip _openSound;
    [Range(0f, 1f)][SerializeField] private float _soundVolume = 1f;
    [SerializeField] private UnityEvent _onOpened;
    [SerializeField] private float _openedAngle = 230f;

    [SerializeField] private float _openDuration = 1.5f;
    private bool _isOpening;
    private System.Action<Empty> _openHandler;

    private void Awake()
    {
        _openHandler = _ => Open();
    }

    private void OnEnable()
    {
        if (_eventChannel != null)
            _eventChannel.OnEventRaised += _openHandler;
    }

    private void OnDestroy()
    {
        if (_eventChannel != null)
            _eventChannel.OnEventRaised -= _openHandler;
    }

    public void Open()
    {
        if (_isOpening) return;
        StartCoroutine(OpenRoutine());
    }

    private IEnumerator OpenRoutine()
    {
        _isOpening = true;
        IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_openSound, transform.position, _soundVolume);
        float startAngle = _door.transform.eulerAngles.y;
        float elapsed = 0f;

        while (elapsed < _openDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _openDuration);
            float angle = Mathf.LerpAngle(startAngle, _openedAngle, t);
            Vector3 euler = _door.transform.eulerAngles;
            euler.y = angle;
            _door.transform.eulerAngles = euler;
            yield return null;
        }

        Vector3 final = _door.transform.eulerAngles;
        final.y = _openedAngle;
        _door.transform.eulerAngles = final;
        _light.SetActive(true);
        _onOpened?.Invoke();
    }
}
