using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPuzzleElement : StatefulPuzzleElement
{
    private bool _triggered;

    public override bool IsInTargetState => _triggered;

    public void Trigger()
    {
        if (_triggered) return;
        _triggered = true;
        RaiseStateChanged();
    }

    public override void ResetElement()
    {
        _triggered = false;
    }
}
