using System.Collections;
using UnityEngine;

public class CrystalGlow : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Color _glowColor = Color.cyan;
    [SerializeField] private float _targetIntensity = 3f;
    [SerializeField] private float _duration = 1.5f;
    [SerializeField] private GameObject[] _glowObjects;

    private Material _material;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        if (_renderer != null)
        {
            _material = _renderer.material;
            _material.SetColor(EmissionColor, Color.black);
        }

        foreach (var obj in _glowObjects)
            if (obj != null) obj.SetActive(false);
    }

    public void Activate()
    {
        StartCoroutine(GlowCoroutine());

        foreach (var obj in _glowObjects)
            if (obj != null) obj.SetActive(true);
    }

    private IEnumerator GlowCoroutine()
    {
        if (_material == null) yield break;

        float elapsed = 0f;
        Color targetColor = _glowColor * _targetIntensity;

        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _duration;
            _material.SetColor(EmissionColor, Color.Lerp(Color.black, targetColor, t));
            yield return null;
        }

        _material.SetColor(EmissionColor, targetColor);
    }
}
