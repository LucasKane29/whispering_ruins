using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AmbientZone : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;
    [Range(0f, 1f)][SerializeField] private float _targetVolume = 0.5f;
    [SerializeField] private float _fadeDuration = 1.5f;

    private AudioSource _audioSource;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = _clip;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;
        _audioSource.volume = 0f;
        _audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartFade(_targetVolume);
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        StartFade(0f, stopOnComplete: true);
    }

    private void StartFade(float target, bool stopOnComplete = false)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeRoutine(target, stopOnComplete));
    }

    private IEnumerator FadeRoutine(float target, bool stopOnComplete)
    {
        float start = _audioSource.volume;
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(start, target, elapsed / _fadeDuration);
            yield return null;
        }

        _audioSource.volume = target;
        if (stopOnComplete)
            _audioSource.Stop();
    }
}
