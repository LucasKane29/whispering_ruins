using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIService : MonoBehaviour, IInventoryUIService
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Transform _slotsParent;
    [SerializeField] private InventorySlotUI _slotPrefab;
    [SerializeField] private TMP_Text _promptText;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private string _title;

    private Action<ItemData> _onPickedCallback;
    private Action _onCancelCallback;
    private bool _selectionMode;
    private InventoryService _inventoryService;
    public event Action OnInventoryOpened;
    public event Action OnInventoryClosed;
    private bool _previousCursorVisible;
    private CursorLockMode _previousCursorLockState;
    void OnEnable()
    {
        if (_inventoryService != null)
            _inventoryService.OnInventoryChanged += Refresh;
    }

    void OnDisable()
    {
        if(_inventoryService != null)
            _inventoryService.OnInventoryChanged -= Refresh;
    }
    public bool IsOpen => _inventoryPanel.activeSelf;

    void Update()
    {
        if(_inventoryPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if(_selectionMode)
            {
                _onCancelCallback?.Invoke();
                Close();
            }
            else
            {
                Close();
            }
        }
    }

    private void OnDestroy()
    {
        if (IServiceLocator.Instance != null)
        {
            IServiceLocator.Instance.TryUnregisterService<IInventoryUIService>(this);
        }
    }

    private void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<IInventoryUIService>(this);
        _inventoryService = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        _inventoryPanel.SetActive(false);
        _cancelButton.onClick.AddListener(HandleCancel);
    }

    public void Open()
    {
        ShowCursor();
        _selectionMode = false;
        _promptText.text = _title;
        _inventoryPanel.SetActive(true);
        Refresh();
        OnInventoryOpened?.Invoke();
    }

    public void OpenForSelection(string prompt, Action<ItemData> onPicked, Action onCancel)
    {
        ShowCursor();
        _selectionMode = true;
        _promptText.text = prompt;
        _onPickedCallback = onPicked;
        _onCancelCallback = onCancel;
        _promptText.gameObject.SetActive(true);
        _inventoryPanel.SetActive(true);
        OnInventoryOpened?.Invoke();
        Refresh();
    }

    public void Close()
    {
        _inventoryPanel.SetActive(false);
        _selectionMode = false;
        _onCancelCallback = null;
        _onPickedCallback = null;
        OnInventoryClosed?.Invoke();
        RestoreCursor();
    }

    private void HandleCancel()
    {
        Close();
        _onCancelCallback?.Invoke();
    }

    public void HandleSlotClicked(ItemData itemData)
    {
        if (_selectionMode && _onPickedCallback != null)
        {
            _onPickedCallback?.Invoke(itemData);
        }
    }

    private void Refresh()
    {
        _inventoryService ??= IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;

        foreach (Transform child in _slotsParent)
            Destroy(child.gameObject);

        if (_inventoryService == null) return;

        foreach (var itemData in _inventoryService.Slots)
        {
            var ui = Instantiate(_slotPrefab, _slotsParent);
            ui.Setup(itemData.Item, itemData.count);
        }
    }

    private void ShowCursor()
    {
        _previousCursorVisible = Cursor.visible;
        _previousCursorLockState = Cursor.lockState;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void RestoreCursor()
    {
        Cursor.visible = _previousCursorVisible;
        Cursor.lockState = _previousCursorLockState;
    }
}
