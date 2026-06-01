using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UseTarget : StatefulPuzzleElement, IInteractable
{
    [SerializeField] private List<ItemData> _acceptedItems = new();
    [SerializeField] private bool _consumeWrongItem = false;
    [SerializeField] private bool _oneTimeUse = true;

    [SerializeField] private AudioClip _wrongItemSound;
    [SerializeField] private AudioClip _correctItemSound;
    [SerializeField] private AudioClip _insertSound;
    [Range(0f, 1f)][SerializeField] private float _soundVolume = 1f;

    [SerializeField] private string _promptText = string.Empty;
    [SerializeField] private string _interactHintText = string.Empty;
    [SerializeField] private string _retrieveHintText = "Take item";
    [SerializeField] private UnityEvent<ItemData> _onCorrectItemUsed;
    [SerializeField] private UnityEvent<ItemData> _onWrongItemUsed;
    [SerializeField] private UnityEvent<ItemData> _onItemRetrieved;
    [SerializeField] private UnityEvent<GameObject> _onItemInsertedIntoSocket;
    [SerializeField] private UnityEvent _onCancelled;
    [SerializeField] private List<RoomCondition> _conditions = new();
    [SerializeField] private Transform _socketPoint;
    [SerializeField] private ItemData _preplacedItem;

    private Outline _outline;
    private bool _used;
    private ItemData _insertedItem;
    private GameObject _spawnedPrefab;
    private InventoryUIService _inventoryUIService;

    private bool CanRetrieve => _insertedItem != null && !_used;

    public bool CanInteract => _conditions.All(c => c.IsMet) &&
        (_insertedItem != null ? CanRetrieve : !(_used && _oneTimeUse));

    public string InteractionText
    {
        get
        {
            foreach (var condition in _conditions)
                if (!condition.IsMet) return condition.FailedHint;
            if (_insertedItem != null) return CanRetrieve ? _retrieveHintText : string.Empty;
            return (_used && _oneTimeUse) ? string.Empty : _interactHintText;
        }
    }

    public override bool IsInTargetState => _used;

    public string InteractHint()
    {
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        if (_insertedItem != null) return CanRetrieve ? _retrieveHintText : string.Empty;
        if (_used && _oneTimeUse) return string.Empty;
        return _interactHintText;
    }

    public void Interact()
    {
        if (!_conditions.All(c => c.IsMet)) return;
        if (CanRetrieve)
        {
            RetrieveItem();
            return;
        }
        if (_used && _oneTimeUse) return;
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        if (inventory == null || inventory.Slots.Count == 0) return;
        _inventoryUIService?.OpenForSelection(_promptText, OnItemPicked, OnCancelled);
    }

    void Start()
    {
        _inventoryUIService = IServiceLocator.Instance.GetService<IInventoryUIService>() as InventoryUIService;

        if (_preplacedItem != null && _socketPoint != null && _socketPoint.childCount > 0)
        {
            _insertedItem = _preplacedItem;
            _spawnedPrefab = _socketPoint.GetChild(0).gameObject;
            foreach (var col in _spawnedPrefab.GetComponentsInChildren<Collider>(true))
                col.enabled = false;
        }
    }

    private void OnItemPicked(ItemData item)
    {
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;

        if (_acceptedItems.Contains(item))
        {
            inventory.Remove(item, 1);
            _used = true;
            RaiseStateChanged();
            if (_correctItemSound != null)
                IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_correctItemSound, transform.position, _soundVolume);
            _onCorrectItemUsed?.Invoke(item);
            _insertedItem = item;
            SpawnItemPrefab(item);
            _spawnedPrefab?.GetComponent<ItemSocketEvents>()?.OnInserted();
        }
        else
        {
            inventory.Remove(item, 1);
            if (!_consumeWrongItem)
            {
                _insertedItem = item;
                SpawnItemPrefab(item);
            }
            if (_wrongItemSound != null)
                IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_wrongItemSound, transform.position, _soundVolume);
            _onWrongItemUsed?.Invoke(item);
        }

        _inventoryUIService?.Close();
    }

    private void SpawnItemPrefab(ItemData item)
    {
        if (_socketPoint == null || item.prefab == null) return;
        _spawnedPrefab = Instantiate(item.prefab, _socketPoint.position, _socketPoint.rotation, _socketPoint);
        foreach (var col in _spawnedPrefab.GetComponentsInChildren<Collider>(true))
            col.enabled = false;
        if (_insertSound != null)
            IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_insertSound, transform.position, _soundVolume);
        _onItemInsertedIntoSocket?.Invoke(_spawnedPrefab);
    }

    private void DestroyItemPrefab()
    {
        if (_spawnedPrefab != null)
        {
            if (_outline != null) _outline.enabled = false;
            _spawnedPrefab.transform.SetParent(null);
            Destroy(_spawnedPrefab);
            _spawnedPrefab = null;
            _outline?.RefreshRenderers();
        }
    }

    private void RetrieveItem()
    {
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        var item = _insertedItem;
        _insertedItem = null;
        DestroyItemPrefab();
        inventory.Add(item, 1);
        if (_used)
        {
            _used = false;
            RaiseStateChanged();
        }
        _onItemRetrieved?.Invoke(item);
    }

    private void OnCancelled()
    {
        _onCancelled?.Invoke();
    }

    private void Awake()
    {
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.enabled = false;
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 10f;
    }

    public void OnFocus()
    {
        if (!ReferenceEquals(_spawnedPrefab, null) && _spawnedPrefab == null)
        {
            _outline.enabled = false;
            _spawnedPrefab = null;
            _insertedItem = null;
        }
        _outline.enabled = !string.IsNullOrEmpty(InteractHint());
    }

    public void OnFocusLost()
    {
        _outline.enabled = false;
    }

    public override void ResetElement()
    {
        _used = false;
        _insertedItem = null;
        DestroyItemPrefab();
    }
}
