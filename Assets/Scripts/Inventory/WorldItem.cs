using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData _item;
    [SerializeField] private int _amount = 1;
    [SerializeField] private string _interactHintText = string.Empty;
    private Outline _outline;

    public bool CanInteract => true;

    public string InteractionText => _interactHintText;

    public void Interact()
    {
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>() as InventoryService;
        if (inventory == null)
            return;
        inventory.Add(_item, _amount);
        gameObject.SetActive(false);
    }

    public string InteractHint()
    {
        return _interactHintText;
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
        _outline.enabled = true;
    }

    public void OnFocusLost()
    {
        _outline.enabled = false;
    }
}
