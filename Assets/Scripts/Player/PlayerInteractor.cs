using UnityEngine;

public struct InteractableItem
{
    public IInteractable Interactable;
    public Transform Transform;
}

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float _interactRange = 2f;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private PlayerInputs _playerInputs;
    private InteractableHint _interactableHint;
    private InventoryService _inventory;

    public ItemData SelectedItem { get; private set; }

    public void SelectItem(ItemData item) => SelectedItem = item;

    public void ClearSelection() => SelectedItem = null;

    private InteractableItem _currentTarget;

    private void Start()
    {
        _interactableHint = IServiceLocator.Instance.GetService<IInteractableHintService>() as InteractableHint;
    }

    void Update()
    {
        InteractableItem nearest = DetectInteractable();
        UpdateFocus(nearest);
        if(_currentTarget.Interactable != null)
        {
            _currentTarget.Interactable.OnFocus();
            if (_playerInputs.interact)
            {
                _playerInputs.interact = false;
                _currentTarget.Interactable.Interact();
            }
            else
            {
                _currentTarget.Interactable?.InteractHint();
            }
        }
    }

    private InteractableItem DetectInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _interactRange, _interactableLayer);
        foreach (Collider collider in colliders)
        {
            if(collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                Transform anchor = collider.transform.Find("HintAnchor") ?? collider.transform;
                return new InteractableItem() { Interactable = interactable, Transform = anchor };
            }
        }
        return new InteractableItem() { Interactable = null, Transform = null};
    }

    private void UpdateFocus(InteractableItem newTarget)
    {
        if(ReferenceEquals(_currentTarget, newTarget)) return;
        _currentTarget.Interactable?.OnFocusLost();
        _currentTarget = newTarget;
        if (_currentTarget.Interactable != null)
        {
            _currentTarget.Interactable?.OnFocus();
            _interactableHint.Show(_currentTarget.Interactable, _currentTarget.Transform);
        }else
        {  
            _interactableHint.Hide();
        }
    }
}
