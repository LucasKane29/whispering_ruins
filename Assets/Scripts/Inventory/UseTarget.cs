using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class UseTarget : StatefulPuzzleElement, IInteractable
{
    [SerializeField] private List<ItemData> _acceptedItems = new();
    [SerializeField] private bool _consumeItem = true;
    [SerializeField] private bool _oneTimeUse = true;

    [SerializeField] private AudioClip _wrongItemSound;
    [SerializeField] private AudioClip _correctItemSound;

    [SerializeField] private string _promptText = string.Empty;
    [SerializeField] private string _interactHintText = string.Empty;
    [SerializeField] private UnityEvent<ItemData> _onCorrectItemUsed;
    [SerializeField] private UnityEvent<ItemData> _onWrongItemUsed;
    [SerializeField] private UnityEvent _onCancelled;
    [SerializeField] private List<RoomCondition> _conditions = new();

    private Outline _outline;

    private bool _used;
    private InventoryUIService _inventoryUIService;

    public bool CanInteract => !(_used && _oneTimeUse) && _conditions.All(c => c.IsMet);

    public string InteractionText => (_used && _oneTimeUse) ? string.Empty : _interactHintText;

    public override bool IsInTargetState => _used;

    public string InteractHint()
    {
        if (_used && _oneTimeUse) return string.Empty;
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        return _interactHintText;
    }

    public void Interact()
    {
        if (_used && _oneTimeUse) return;
        if (!_conditions.All(c => c.IsMet)) return;
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        if (inventory == null || inventory.Slots.Count == 0) return;
        _inventoryUIService?.OpenForSelection(_promptText, OnItemPicked, OnCancelled);
    }

    void Start()
    {
        _inventoryUIService = IServiceLocator.Instance.GetService<IInventoryUIService>() as InventoryUIService;
    }

    private void OnItemPicked(ItemData item)
    {
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        if (_acceptedItems.Contains(item))
        {
            if(_consumeItem)
                inventory.Remove(item, 1);
            _used = true;
            RaiseStateChanged();
            if (_correctItemSound != null)
                AudioSource.PlayClipAtPoint(_correctItemSound, transform.position);
            _onCorrectItemUsed?.Invoke(item);
            _inventoryUIService?.Close();
        }
        else
        {
            if(_wrongItemSound != null)
                AudioSource.PlayClipAtPoint(_wrongItemSound, transform.position);
            _onWrongItemUsed?.Invoke(item);
        }

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
        _outline.enabled = !(_used && _oneTimeUse);
    }

    public void OnFocusLost()
    {
        _outline.enabled = false;
    }

    public override void ResetElement()
    {
        _used = false;
    }
}
