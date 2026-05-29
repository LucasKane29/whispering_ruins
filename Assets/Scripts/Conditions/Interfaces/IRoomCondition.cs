using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRoomCondition
{
    public bool IsMet { get; }
    public string FailedHint { get; }
}
    