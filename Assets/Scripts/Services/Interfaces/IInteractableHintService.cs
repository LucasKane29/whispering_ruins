using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractableHintService : IService
{
    public void Show(IInteractable interactable, Transform transform);
    public void Hide();
}
