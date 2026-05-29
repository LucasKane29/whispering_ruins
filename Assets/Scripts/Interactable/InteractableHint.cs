using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractableHint : MonoBehaviour, IInteractableHintService
{
    [SerializeField] private TMP_Text _hintText;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 1.5f, 0f);
    [SerializeField] private InputActionAsset _playerInput;
    private Camera _camera;
    private Transform _target;
    private Canvas _canvas;
    private RectTransform _canvasRect;
    private RectTransform _labelRect;
    private string _bindedKey;
    private CameraService _cameraService;

    void Awake()
    {
        _canvas = _hintText.GetComponentInParent<Canvas>();
        _canvasRect = _canvas.GetComponent<RectTransform>();
        Hide();

        _bindedKey = _playerInput.FindAction("Interact").bindings[0].ToDisplayString();
        IServiceLocator.Instance.TryRegisterService<IInteractableHintService, InteractableHint>(this);
    }

    private void Start()
    {
        _cameraService = IServiceLocator.Instance.GetService<CameraService>();
        if (_camera == null)
        {
            _camera = _cameraService?.Camera;
        }
    }

    private void LateUpdate()
    {
        if(_camera == null)
        {
            _cameraService = IServiceLocator.Instance.GetService<CameraService>();   
            _camera = _cameraService?.Camera;
        }

        if (_target == null || _camera == null) 
            return;
        if (!_hintText.gameObject.activeSelf)
        {
            _hintText.gameObject.SetActive(true);
        }
        Vector3 worldPos = _target.position + _offset;
        Vector3 screenPos = _camera.WorldToScreenPoint(worldPos);
        Camera uiCam = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _camera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPos, uiCam, out Vector2 localPoint))
        {
            ((RectTransform)this.transform).anchoredPosition = localPoint;
        }
    }

    public void Show(IInteractable interactable, Transform transform)
    {
        if(interactable == null) {
            Hide();
            return;
        }

        string hint = interactable.InteractHint();

        if(string.IsNullOrEmpty(hint))
        {
            Hide();
            return;
        }

        _target = transform;
        if (interactable.CanInteract)
            _hintText.text = $"[{_bindedKey}]{interactable.InteractHint()}";
        else
            _hintText.text = interactable.InteractHint();

        _hintText.gameObject.SetActive(true);
        this.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(10, 10, 10, 10);

    }

    public void Hide()
    {
        _hintText.text = string.Empty;
        _hintText.gameObject.SetActive(false);
        _target = null;
        this.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(0, 0, 0, 0);
    }
}
