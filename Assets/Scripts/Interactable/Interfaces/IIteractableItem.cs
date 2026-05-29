using UnityEngine;

public interface IIteractableItem : IInteractable
{
    void Interact(GameObject interactor);
}
