using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioClip _hoverSound;
    [SerializeField] private AudioClip _clickSound;
    [Range(0f, 1f)][SerializeField] private float _volume = 1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(_hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(_clickSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        IServiceLocator.Instance.GetService<ISoundService>()
            ?.Play2D(clip, _volume);
    }
}
