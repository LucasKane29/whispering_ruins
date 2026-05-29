using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEffect : MonoBehaviour, IInteractionEffect
{
    [SerializeField] private Vector3 _axis = Vector3.right;
    [SerializeField] private float _targetAngle = 90f;
    [SerializeField] private float _duration = 1f;
    [SerializeField] private AnimationCurve _easeCurve;
    [SerializeField] private AudioClip _rotateSound;

    private AudioSource _audioSource;
    private bool _isRotating;

    public void Execute()
    {
        if (!_isRotating)
        {
            _isRotating = true;
            StartCoroutine(RotateCoroutine());
        }
    }

    private IEnumerator RotateCoroutine()
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.AngleAxis(_targetAngle, _axis);
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
    }
}
