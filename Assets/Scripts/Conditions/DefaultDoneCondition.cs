using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultDoneCondition : RoomCondition
{
    [SerializeField] private string _failedHint;
    public override bool IsMet => true;
    public override string FailedHint => _failedHint;
}
