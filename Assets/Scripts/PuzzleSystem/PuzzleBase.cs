using System;
using UnityEngine;
using UnityEngine.Events;

public enum PuzzleState { 
    Idle, 
    Active, 
    Solved, 
    Failed 
}

public abstract class PuzzleBase : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] protected PuzzleConfig config;
    [SerializeField] protected PuzzleElement[] elements;
    [SerializeField] protected PuzzleReward[] rewards;

    [Header("Events")]
    public UnityEvent OnPuzzleStarted;
    public UnityEvent OnPuzzleSolved;
    public UnityEvent OnPuzzleFailed;
    public UnityEvent<float> OnProgressChanged;

    public PuzzleState State { get; protected set; } = PuzzleState.Idle;
    public event Action<PuzzleState> StateChanged;

    protected virtual void Awake() 
    {
        foreach (var element in elements)
            element.Initialize(this);
    }

    public virtual void StartPuzzle()
    {
        if (State == PuzzleState.Solved) return;
        ChangeState(PuzzleState.Active);
        OnPuzzleStarted?.Invoke();
    }

    public abstract void OnElementInteracted(PuzzleElement element, object data);

    protected virtual void SolvePuzzle()
    {
        ChangeState(PuzzleState.Solved);
        OnPuzzleSolved?.Invoke();
        foreach (var reward in rewards)
            reward.Give();
    }

    protected virtual void FailPuzzle()
    {
        ChangeState(PuzzleState.Failed);
        OnPuzzleFailed?.Invoke();
        ResetPuzzle();
    }

    public virtual void ResetPuzzle()
    {
        foreach (var element in elements)
            element.ResetElement();
        ChangeState(PuzzleState.Idle);
    }

    protected void ChangeState(PuzzleState newState)
    {
        State = newState;
        StateChanged?.Invoke(newState);
    }
}
