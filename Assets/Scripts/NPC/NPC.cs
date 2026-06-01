using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private EventChannel _eventChannel;
    [SerializeField] private DialogueSO _dialogue;

    [SerializeField] private string _interactText;
    [SerializeField] private List<RoomCondition> _conditions;
    private Outline _outline;

    public bool CanInteract => _conditions.All(c => c.IsMet);

    public string InteractionText => _interactText;

    private void Awake()
    {
        _outline = GetComponent<Outline>() ?? gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.enabled = false;
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 10f;
    }

    public void Interact()
    {
        if (CanInteract)
        {
            if (DialogueManager.Instance.IsActive)
            {
                DialogueManager.Instance.Skip();
                return;
            }
            DialogueManager.Instance.StartDialogue(_dialogue);
            DialogueManager.Instance.OnDialogueEnded += OnDialogueEnd;
        }
    }

    public string InteractHint()
    {
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        return _interactText;
    }

    public void OnDialogueEnd()
    {
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnd;
        _eventChannel?.Invoke(new Empty());
    }

    public void SetDialogue(DialogueSO dialogue) => _dialogue = dialogue;

    public void OnFocus() => _outline.enabled = true;
    public void OnFocusLost() => _outline.enabled = false;
}
