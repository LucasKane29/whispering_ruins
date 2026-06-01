using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    private ISoundService _soundService;

    private void Start()
    {
        _soundService = IServiceLocator.Instance.GetService<ISoundService>();
        if (_soundService == null) return;

        _musicSlider.SetValueWithoutNotify(_soundService.MusicVolume);
        _sfxSlider.SetValueWithoutNotify(_soundService.SfxVolume);

        _musicSlider.onValueChanged.AddListener(_soundService.SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(_soundService.SetSfxVolume);
    }

    private void OnDestroy()
    {
        _musicSlider.onValueChanged.RemoveListener(_soundService.SetMusicVolume);
        _sfxSlider.onValueChanged.RemoveListener(_soundService.SetSfxVolume);
    }
}
