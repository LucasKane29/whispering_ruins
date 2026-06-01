using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip _musicClip;
    [SerializeField] private float _fadeDuration = 1f;
    [Range(0f, 1f)][SerializeField] private float _volume = 1f;

    private void Start()
    {
        if (_musicClip == null) return;
        IServiceLocator.Instance.GetService<ISoundService>()?.PlayMusic(_musicClip, _fadeDuration, volume: _volume);
    }
}
