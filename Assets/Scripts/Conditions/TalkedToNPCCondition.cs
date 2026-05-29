using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkedToNPCCondition : RoomCondition
{
    [SerializeField] private string _failedHint;
    private bool _hasTalkedToNPC;
    public override bool IsMet => _hasTalkedToNPC;
    public override string FailedHint => _failedHint;

    public void OnTalkedToNPC()
    {
        _hasTalkedToNPC = true;
    }
}
