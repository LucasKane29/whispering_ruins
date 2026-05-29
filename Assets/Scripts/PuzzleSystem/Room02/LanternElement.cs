using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LanternElement : StatefulPuzzleElement, IInteractable
{
    [SerializeField] private Color[] _availableColors;
    [SerializeField] private Color _targetColor;
    [SerializeField] private Light _light;
    [SerializeField] private string _interactText;
    [SerializeField] private List<RoomCondition> _conditions = new();
    [SerializeField] private GameObject _outlineTarget;

    [SerializeField] private Outline _outline;
    private int _currentColorIndex;
    private int _targetColorIndex;

    public bool CanInteract => !IsInTargetState && _conditions.All(c => c.IsMet);
    public string InteractionText => _interactText;
    public override bool IsInTargetState => _currentColorIndex == _targetColorIndex;

    private void Start()
    {
        _outline.enabled = false;
        _targetColorIndex = FindColorIndex(_targetColor);
        RandomizeColor();
    }

    public void Interact()
    {
        if (!CanInteract) return;
        _currentColorIndex = (_currentColorIndex + 1) % _availableColors.Length;
        ApplyColor();
        RaiseStateChanged();
    }

    public string InteractHint()
    {
        if (IsInTargetState) return string.Empty;
        foreach (var condition in _conditions)
            if (!condition.IsMet) return condition.FailedHint;
        return _interactText;
    }
    public void OnFocus() => _outline.enabled = !IsInTargetState;
    public void OnFocusLost() => _outline.enabled = false;

    public override void ResetElement()
    {
        RandomizeColor();
    }

    private void RandomizeColor()
    {
        if (_availableColors.Length < 2) return;
        do
        {
            _currentColorIndex = UnityEngine.Random.Range(0, _availableColors.Length);
        }
        while (_currentColorIndex == _targetColorIndex);
        ApplyColor();
    }

    private void ApplyColor()
    {
        if (_light != null)
            _light.color = _availableColors[_currentColorIndex];
    }

    private int FindColorIndex(Color target)
    {
        Color32 target32 = target;
        for (int i = 0; i < _availableColors.Length; i++)
        {
            Color32 c = _availableColors[i];
            if (c.r == target32.r && c.g == target32.g &&
                c.b == target32.b && c.a == target32.a)
                return i;
        }
        return 0;
    }
}
