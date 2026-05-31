using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatuePuzzleElement : StatefulPuzzleElement, IInteractable
{
    [SerializeField] private RotateEffect _rotateEffect;
    [SerializeField] private Transform _crystal;
    [SerializeField] private LayerMask _crystalLayer;
    [SerializeField] private float _maxRayDistance = 20f;
    [SerializeField] private float _raycastHeight = 1f;
    [SerializeField] private Transform _eyePoint;
    [SerializeField] private string _interactHintText = string.Empty;
    [SerializeField] private List<RoomCondition> _conditions = new();

    private Outline _outline;
    private Quaternion _initialRotation;

    private Vector3 RayOrigin => transform.position + Vector3.up * _raycastHeight;
    private Vector3 RayDirection => _eyePoint != null ? _eyePoint.forward : transform.forward;

    public bool CanInteract => (ParentPuzzle == null || ParentPuzzle.State != PuzzleState.Solved) && _conditions.All(c => c.IsMet);
    public string InteractionText => _interactHintText;

    public override bool IsInTargetState
    {
        get
        {
            if (_crystal == null) return false;
            return Physics.Raycast(RayOrigin, RayDirection, out RaycastHit hit, _maxRayDistance, _crystalLayer)
                && hit.transform == _crystal;
        }
    }

    private void Awake()
    {
        _initialRotation = transform.rotation;
        _outline = gameObject.AddComponent<Outline>();
        _outline.OutlineMode = Outline.Mode.OutlineVisible;
        _outline.enabled = false;
        _outline.OutlineColor = Color.yellow;
        _outline.OutlineWidth = 10f;
    }

    public override void Initialize(PuzzleBase puzzle)
    {
        base.Initialize(puzzle);
        if (_rotateEffect != null)
            _rotateEffect.OnRotationComplete += OnRotationFinished;
    }

    private void OnDestroy()
    {
        if (_rotateEffect != null)
            _rotateEffect.OnRotationComplete -= OnRotationFinished;
    }

    private void OnRotationFinished() => RaiseStateChanged();

    public void Interact()
    {
        if (!CanInteract) return;
        _rotateEffect?.Execute();
    }

    public string InteractHint()
    {
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        if (ParentPuzzle != null && ParentPuzzle.State == PuzzleState.Solved) return string.Empty;
        return _interactHintText;
    }

    public void OnFocus() => _outline.enabled = !IsInTargetState;

    public void OnFocusLost() => _outline.enabled = false;

    public override void ResetElement()
    {
        _rotateEffect?.ResetRotation(_initialRotation);
    }

    private void OnDrawGizmos()
    {
        bool hitting = _crystal != null
            && Physics.Raycast(RayOrigin, RayDirection, out RaycastHit hit, _maxRayDistance, _crystalLayer)
            && hit.transform == _crystal;

        Gizmos.color = hitting ? Color.green : Color.red;
        Gizmos.DrawRay(RayOrigin, RayDirection * _maxRayDistance);

        if (_crystal != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(RayOrigin, _crystal.position);
        }
    }
}
