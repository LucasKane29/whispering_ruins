using System.Collections;
using UnityEngine;

public class SoundService : MonoBehaviour, ISoundService
{

    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;

    void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<ISoundService, SoundService>(this);
    }

    void OnDestroy()
    {
        if (IServiceLocator.Instance != null)
        {
            IServiceLocator.Instance.TryUnregisterService<ISoundService, SoundService>(this);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 0f)
    {
        if (_musicSource.isPlaying)
            StopMusic(0f); // або можна зробити crossfade пізніше

        _musicSource.clip = clip;
        _musicSource.volume = fadeDuration > 0f ? 0f : 1f;
        _musicSource.Play();

        if (fadeDuration > 0f)
            StartCoroutine(FadeVolume(_musicSource, 0f, 1f, fadeDuration));
    }

    public void StopMusic(float fadeDuration = 0f)
    {
        if (!_musicSource.isPlaying) return;

        if (fadeDuration > 0f)
            StartCoroutine(FadeAndStop(_musicSource, fadeDuration));
        else
            _musicSource.Stop();
    }

    public void PlayOneShot(AudioClip clip, Vector3 position, float volume = 1f)
    => AudioSource.PlayClipAtPoint(clip, position, volume);

    public void Play2D(AudioClip clip, float volume = 1f)
    {
        _sfxSource.volume = volume;
        _sfxSource.PlayOneShot(clip);
    }

    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        source.volume = to;
    }

    private IEnumerator FadeAndStop(AudioSource source, float duration)
    {
        yield return FadeVolume(source, source.volume, 0f, duration);
        source.Stop();
    }
}
