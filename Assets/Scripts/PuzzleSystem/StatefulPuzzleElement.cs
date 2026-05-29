using System;
using UnityEngine;

public abstract class StatefulPuzzleElement : PuzzleElement
{
    public abstract bool IsInTargetState { get; }

    protected void NotifyStateChanged()
    {
        NotifyPuzzle();
    }

    public event Action StateChanged;

    protected void RaiseStateChanged()
    {
        StateChanged?.Invoke();
        NotifyStateChanged();
    }
}
