using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO _dialogue;

    public void Interact()
    {
        if (DialogueManager.Instance.IsActive)
            return;
        DialogueManager.Instance.StartDialogue(_dialogue);
    }
}
