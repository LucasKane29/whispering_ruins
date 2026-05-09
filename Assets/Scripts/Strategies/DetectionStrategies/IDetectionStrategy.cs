using UnityEngine;

public partial class PlayerDetector
{
    public interface IDetectionStrategy
    {
        public bool Execute(Transform player, Transform detector, CountdownTimer timer);
    }
}
