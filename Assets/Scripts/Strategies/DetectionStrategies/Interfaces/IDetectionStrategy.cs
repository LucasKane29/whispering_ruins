using UnityEngine;

public interface IDetectionStrategy
{
    public bool Execute(Transform player, Transform detector, CountdownTimer timer);
}
