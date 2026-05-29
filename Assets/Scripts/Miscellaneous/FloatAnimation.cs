using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [SerializeField] private float _amplitude = 0.3f;
    [SerializeField] private float _frequency = 1f;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        float offset = (Mathf.Sin(Time.time * _frequency) + 1f) * 0.5f * _amplitude;
        transform.position = _startPosition + Vector3.up * offset;
    }
}
