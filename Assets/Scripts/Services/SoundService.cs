using System.Collections;
using UnityEngine;

public class SoundService : MonoBehaviour, ISoundService
{
    [SerializeField] private AudioSource _sfxSource;
    [SerializeField] private AudioSource _musicSource;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey   = "SfxVolume";

    private float _masterMusicVolume = 1f;
    private float _masterSfxVolume   = 1f;
    private float _currentClipVolume = 1f;

    private Coroutine _musicCoroutine;
    private AudioClip _currentClip;

    public float MusicVolume => _masterMusicVolume;
    public float SfxVolume   => _masterSfxVolume;

    void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<ISoundService, SoundService>(this);
        _masterMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        _masterSfxVolume   = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
    }

    void OnDestroy()
    {
        if (IServiceLocator.Instance != null)
        {
            IServiceLocator.Instance.TryUnregisterService<ISoundService, SoundService>(this);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 0f, bool loop = true, float volume = 1f)
    {
        if (_currentClip == clip) return;

        CancelMusicCoroutine();
        _currentClip = clip;
        _currentClipVolume = volume;

        float finalVolume = volume * _masterMusicVolume;
        if (fadeDuration > 0f && _musicSource.isPlaying)
            _musicCoroutine = StartCoroutine(CrossfadeMusic(clip, loop, finalVolume, fadeDuration));
        else
        {
            _musicSource.Stop();
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = finalVolume;
            _musicSource.Play();
        }
    }

    public void SetMusicVolume(float volume)
    {
        _masterMusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MusicVolumeKey, _masterMusicVolume);
        if (_musicSource.isPlaying)
            _musicSource.volume = _currentClipVolume * _masterMusicVolume;
    }

    public void SetSfxVolume(float volume)
    {
        _masterSfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SfxVolumeKey, _masterSfxVolume);
    }

    public void StopMusic(float fadeDuration = 0f)
    {
        if (!_musicSource.isPlaying) return;

        CancelMusicCoroutine();

        if (fadeDuration > 0f)
            _musicCoroutine = StartCoroutine(FadeAndStop(_musicSource, fadeDuration));
        else
            _musicSource.Stop();
    }

    public void PlayOneShot(AudioClip clip, Vector3 position, float volume = 1f)
        => AudioSource.PlayClipAtPoint(clip, position, volume * _masterSfxVolume);

    public void Play2D(AudioClip clip, float volume = 1f)
    {
        _sfxSource.volume = volume * _masterSfxVolume;
        _sfxSource.PlayOneShot(clip);
    }

    private void CancelMusicCoroutine()
    {
        if (_musicCoroutine == null) return;
        StopCoroutine(_musicCoroutine);
        _musicCoroutine = null;
    }

    private IEnumerator FadeVolume(AudioSource source, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
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

    private IEnumerator CrossfadeMusic(AudioClip newClip, bool loop, float volume, float duration)
    {
        float half = duration * 0.5f;
        yield return FadeVolume(_musicSource, _musicSource.volume, 0f, half);
        _musicSource.Stop();
        _musicSource.clip = newClip;
        _musicSource.loop = loop;
        _musicSource.volume = 0f;
        _musicSource.Play();
        yield return FadeVolume(_musicSource, 0f, volume, half);
    }
}
