using System.Linq;
using UnityEngine;

public class StatePuzzle : PuzzleBase
{
    public override void OnElementInteracted(PuzzleElement element, object data)
    {
        if (State == PuzzleState.Solved) return;
        if (State != PuzzleState.Active) StartPuzzle();

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        var stateful = elements.OfType<StatefulPuzzleElement>().ToArray();

        int correct = stateful.Count(e => e.IsInTargetState);
        OnProgressChanged?.Invoke((float)correct / stateful.Length);

        if (correct == stateful.Length)
            SolvePuzzle();
    }
}