using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleElement : MonoBehaviour
{
    [SerializeField] private string elementId;
    public string ElementId => elementId;

    protected PuzzleBase ParentPuzzle { get; private set; }

    public virtual void Initialize(PuzzleBase puzzle)
    {
        ParentPuzzle = puzzle;
    }
    protected void NotifyPuzzle(object data = null)
    {
        ParentPuzzle?.OnElementInteracted(this, data);
    }

    public abstract void ResetElement();
}
