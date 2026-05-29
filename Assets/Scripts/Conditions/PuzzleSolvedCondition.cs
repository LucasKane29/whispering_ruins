using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolvedCondition : RoomCondition
{
    [SerializeField] private string _failedHint;
    private bool _hasSolved;
    public override bool IsMet => _hasSolved;
    public override string FailedHint => _failedHint;

    public void OnSolvePuzzle()
    {
        _hasSolved = true;
    }
}
