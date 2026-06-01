using System;
using System.Collections;
using UnityEngine;

public class RotateEffect : MonoBehaviour, IInteractionEffect
{
    [SerializeField] private Vector3 _axis = Vector3.up;
    [SerializeField] private float _targetAngle = 90f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private AnimationCurve _easeCurve;
    [SerializeField] private AudioClip _rotateSound;
    [Range(0f, 1f)][SerializeField] private float _soundVolume = 1f;

    public event Action OnRotationComplete;

    private bool _isRotating;
    private Coroutine _rotateCoroutine;

    public void Execute()
    {
        if (!_isRotating)
        {
            _isRotating = true;
            _rotateCoroutine = StartCoroutine(RotateCoroutine());
        }
    }

    public void ResetRotation(Quaternion targetRotation)
    {
        if (_rotateCoroutine != null)
            StopCoroutine(_rotateCoroutine);
        _isRotating = false;
        transform.rotation = targetRotation;
    }

    private IEnumerator RotateCoroutine()
    {
        IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_rotateSound, transform.position, _soundVolume);
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.AngleAxis(_targetAngle, _axis) * startRot;
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _duration);
            float smoothT = _easeCurve.Evaluate(t);
            transform.rotation = Quaternion.Lerp(startRot, endRot, smoothT);
            yield return null;
        }
        transform.rotation = endRot;
        _isRotating = false;
        OnRotationComplete?.Invoke();
    }
}
