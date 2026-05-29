using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactText;

    [SerializeField] private UnityEvent _onInteract;
    [SerializeField] private List<RoomCondition> _conditions;
    [SerializeField] private List<MonoBehaviour> _effects;
    [SerializeField] private bool _isOneUse = false;
    private Outline _outline;
    private bool _used = false;

    public bool CanInteract => _conditions.All(condition => condition.IsMet) && !_used;

    public string InteractionText => _interactText;

    public void Interact()
    {
        if(_used) return;

        if (!CanInteract) return;
        _onInteract?.Invoke();
        if(_isOneUse)
            _used = true;
        foreach (var effect in _effects)
        {
            (effect as IInteractionEffect)?.Execute();
        }
    }

    public string InteractHint()
    {
        if (_used) return string.Empty;

        if (!CanInteract)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.IsMet)
                {
                    return condition.FailedHint;
                }
            }

        }

        return _interactText;
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
        _outline.enabled = !_used;
    }

    public void OnFocusLost()
    {
        _outline.enabled = false;
    }
}
