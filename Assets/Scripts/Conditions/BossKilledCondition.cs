using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossKilledCondition : RoomCondition
{
    [SerializeField] private string _failedHint;
    private bool _hasSolved;
    public override bool IsMet => _hasSolved;
    public override string FailedHint => _failedHint;

    public void OnBossKilled()
    {
        _hasSolved = true;
    }
}
