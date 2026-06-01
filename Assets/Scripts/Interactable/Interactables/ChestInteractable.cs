using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ChestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private List<ItemData> _items;
    [SerializeField] private string _interactText;

    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] private List<RoomCondition> _conditions;

    [SerializeField] private Transform _lid;
    [SerializeField] private float _openAngleX = 60f;
    [SerializeField] private float _openDuration = 0.6f;
    [SerializeField] private AnimationCurve _openCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _lootSound;
    [Range(0f, 1f)][SerializeField] private float _soundVolume = 1f;

    [SerializeField] private GameObject _contentObject;

    [SerializeField] private Renderer _interiorRenderer;

    [ColorUsage(showAlpha: true, hdr: true)]
    [SerializeField] private Color _emissionColor = Color.yellow;

    [SerializeField] private float _emissionDuration = 0.5f;

    [SerializeField] private Light _interiorLight;
    [SerializeField] private float _targetIntensity = 3f;
    [SerializeField] private float _lightDuration = 0.5f;

    private bool _isOpened = false;
    private Outline _outline;
    private IInventoryService _inventory;

    public bool CanInteract => _conditions.All(condition => condition.IsMet) && !_isOpened;

    public string InteractionText => _interactText;

    public void Interact()
    {
        if (_isOpened || _inventory == null) return;

        if (!CanInteract)
        {
            return;
        }
        StartCoroutine(OpenLidRoutine());
        _onInteract?.Invoke();
    }

    public string InteractHint()
    {
        if (_isOpened) return string.Empty;

        if (!CanInteract)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.IsMet)
                {
                    return condition.FailedHint;
                }
            }

        }

        return _interactText;
    }

    private void Awake()
    {
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.enabled = false;
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 10f;

        _interiorLight.intensity = 0f;
        _interiorLight.enabled = false;
    }

    public void OnFocus()
    {
        _outline.enabled = !_isOpened;
    }

    public void OnFocusLost()
    {
        _outline.enabled = false;
    }

    void Start()
    {
        _inventory = IServiceLocator.Instance.GetService<IInventoryService>();
    }

    private IEnumerator OpenLidRoutine()
    {
        _isOpened = true;
        IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_openSound, transform.position, _soundVolume);
        float elapsed = 0f;
        Quaternion startRot = _lid.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(_openAngleX, 0f, 0f);

        while (elapsed < _openDuration)
        {
            elapsed += Time.deltaTime;
            float t = _openCurve.Evaluate(Mathf.Clamp01(elapsed / _openDuration));
            _lid.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        _lid.localRotation = targetRot;
        StartCoroutine(EmissionRoutine());
        StartCoroutine(LightRoutine());

        foreach (var item in _items)
            _inventory.Add(item);

        IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_lootSound, transform.position, _soundVolume);
        if (_contentObject != null)
            _contentObject.SetActive(false);
    }

    private IEnumerator EmissionRoutine()
    {
        var mpb = new MaterialPropertyBlock();
        float elapsed = 0f;

        while (elapsed < _emissionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _emissionDuration);
            mpb.SetColor("_EmissionColor", Color.Lerp(Color.black, _emissionColor, t));
            _interiorRenderer.SetPropertyBlock(mpb);
            yield return null;
        }

        mpb.SetColor("_EmissionColor", _emissionColor);
        _interiorRenderer.SetPropertyBlock(mpb);
    }

    private IEnumerator LightRoutine()
    {
        _interiorLight.enabled = true;
        float elapsed = 0f;

        while (elapsed < _lightDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _lightDuration);
            _interiorLight.intensity = Mathf.Lerp(0f, _targetIntensity, t);
            yield return null;
        }

        _interiorLight.intensity = _targetIntensity;
    }
}
