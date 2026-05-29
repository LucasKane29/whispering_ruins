using UnityEngine;

public class ConeDetectionStrategy : IDetectionStrategy
{
    private float _detectionAngle;
    private float _detectionRadius;
    private float _innerDetectionRadius;

    public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
    {
        _detectionAngle = detectionAngle;
        _detectionRadius = detectionRadius;
        _innerDetectionRadius = innerDetectionRadius;
    }
    public bool Execute(Transform player, Transform detector, CountdownTimer timer)
    {
        if (timer.IsRunning) return false;

        var directionToPlayer = player.position - detector.position;
        var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

        if ((!(angleToPlayer < _detectionAngle / 2f) || !(directionToPlayer.magnitude < _detectionRadius))
            && !(directionToPlayer.magnitude < _innerDetectionRadius))
        {
            return false;
        }

        timer.Start();

        return true;
    }
}
