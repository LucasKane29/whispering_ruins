using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PuzzleButton : PuzzleElement, IInteractable
{
    [SerializeField] private string _interactHintText = "Press";
    [SerializeField] private Vector3 _pressDirection = Vector3.down;
    [SerializeField] private float _pressDistance = 0.05f;
    [SerializeField] private float _pressDuration = 0.1f;
    [SerializeField] private List<RoomCondition> _conditions = new();

    private Outline _outline;
    private Vector3 _originalLocalPosition;
    private bool _isPressed;
    private Coroutine _animationCoroutine;

    public bool CanInteract => _conditions.All(c => c.IsMet) && ParentPuzzle?.State != PuzzleState.Solved;

    public string InteractionText
    {
        get
        {
            foreach (var condition in _conditions)
                if (!condition.IsMet) return condition.FailedHint;
            return ParentPuzzle?.State != PuzzleState.Solved ? _interactHintText : string.Empty;
        }
    }

    public string InteractHint()
    {
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        return ParentPuzzle?.State != PuzzleState.Solved ? _interactHintText : string.Empty;
    }

    private void Awake()
    {
        _originalLocalPosition = transform.localPosition;
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.enabled = false;
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 10f;
    }

    public void Interact()
    {
        if (!_conditions.All(c => c.IsMet)) return;
        if (ParentPuzzle?.State == PuzzleState.Solved) return;
        NotifyPuzzle();
        Press();
    }

    public void OnFocus() => _outline.enabled = !string.IsNullOrEmpty(InteractHint());
    public void OnFocusLost() => _outline.enabled = false;

    private void Press()
    {
        _isPressed = true;
        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(AnimateTo(
            _originalLocalPosition + _pressDirection * _pressDistance));
    }

    private void Release()
    {
        if (!_isPressed) return;
        _isPressed = false;
        if (_animationCoroutine != null) StopCoroutine(_animationCoroutine);
        _animationCoroutine = StartCoroutine(AnimateTo(_originalLocalPosition));
    }

    private IEnumerator AnimateTo(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float elapsed = 0f;
        while (elapsed < _pressDuration)
        {
            elapsed += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(start, target, elapsed / _pressDuration);
            yield return null;
        }
        transform.localPosition = target;
    }

    public override void ResetElement() => Release();
}
