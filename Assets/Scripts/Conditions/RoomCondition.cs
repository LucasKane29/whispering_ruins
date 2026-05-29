using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoomCondition : MonoBehaviour, IRoomCondition
{
    public abstract bool IsMet { get; }
    public abstract string FailedHint { get; }
}
