using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlockReward : PuzzleReward
{
    [SerializeField] EventChannel _puzzleSolvedCondition;
    public override void Give()
    {
        _puzzleSolvedCondition?.Invoke(new Empty());
    }
}
