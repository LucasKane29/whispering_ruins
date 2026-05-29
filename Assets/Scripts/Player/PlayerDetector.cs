using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private float _detectionAngle = 60f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _innerDetectionRadius = 5f;
    [SerializeField] private float _detectionCooldown = 1f;

    public Transform Player { get; private set; }
    public Health PlayerHealth { get; private set; }
    private CountdownTimer _detectionTimer;

    private IDetectionStrategy _detectionStrategy;

    private IPlayerService _playerService;

    void Start()
    {
        _playerService = IServiceLocator.Instance.GetService<IPlayerService>();
        Player = _playerService.Transform;
        PlayerHealth = _playerService.Health;

        _detectionTimer = new CountdownTimer(_detectionCooldown);
        _detectionStrategy = new ConeDetectionStrategy(_detectionAngle, _detectionRadius, _innerDetectionRadius);
    }

    void Update() => _detectionTimer.Tick(Time.deltaTime);

    public bool CanDetectPlayer()
    {
        if (Player == null || _detectionTimer == null) return false;
        return _detectionTimer.IsRunning || _detectionStrategy.Execute(Player, transform, _detectionTimer);
    }

    public bool CanAttackPlayer(float attackRange)
    {
        if (Player == null) return false;
        var directionToPlayer = Player.position - transform.position;
        return directionToPlayer.magnitude <= attackRange;
    }

    public void SetDetectionStrategy(IDetectionStrategy detectionStrategy) => _detectionStrategy = detectionStrategy;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        Gizmos.DrawWireSphere(transform.position, _innerDetectionRadius);

        Vector3 forwardConeDirection = Quaternion.Euler(0, _detectionAngle / 2f, 0) * transform.forward * _detectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -_detectionAngle / 2f, 0) * transform.forward * _detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }

}
