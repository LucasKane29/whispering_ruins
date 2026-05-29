using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public bool CanInteract { get;}
    public string InteractionText { get;}
    public void Interact();
    public string InteractHint();
    public void OnFocus();
    public void OnFocusLost();
}
