using UnityEngine;
using UnityEngine.AI;

public class BossBurrowState : BaseState<BossController>
{
    private enum Phase { Waiting, Diving, Underground, Warning, Emerging }

    private readonly NavMeshAgent _navMeshAgent;
    private readonly PlayerDetector _playerDetector;
    private readonly float _arrivalThreshold;
    private readonly float _undergroundSpeed;
    private readonly float _emergeSampleRadius;
    private readonly GameObject _warningPrefab;
    private readonly float _warningDuration;
    private readonly float _burrowDelay;
    private GameObject _activeWarning;
    private float _warningRadius;
    private float _warningTimer;
    private float _delayTimer;

    private Phase _phase;
    private bool _emergeStarted;
    private bool _emergeHitDealt;
    public bool IsComplete { get; private set; }

    public BossBurrowState(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
        _playerDetector = playerDetector;
        _arrivalThreshold = agent.ArrivalThreshold;
        _undergroundSpeed = agent.UndergroundSpeed;
        _emergeSampleRadius = agent.EmergeSampleRadius;
        _warningPrefab = agent.BurrowWarningPrefab;
        _warningDuration = agent.BurrowWarningDuration;
        _burrowDelay = agent.BurrowDelay;
    }

    public override void OnEnter()
    {
        IsComplete = false;
        _emergeStarted = false;
        _emergeHitDealt = false;
        _delayTimer = _burrowDelay;
        _phase = Phase.Waiting;
        animator.CrossFade(BossAnimIDs.Idle, crossFadeDuration);
    }

    public override void Update()
    {
        switch (_phase)
        {
            case Phase.Waiting:     UpdateWaiting();     break;
            case Phase.Diving:      UpdateDiving();      break;
            case Phase.Underground: UpdateUnderground(); break;
            case Phase.Warning:     UpdateWarning();     break;
            case Phase.Emerging:    UpdateEmerging();    break;
        }
    }

    private void UpdateWaiting()
    {
        _delayTimer -= Time.deltaTime;
        if (_delayTimer <= 0f)
        {
            _navMeshAgent.enabled = false;
            agent.SetColliderEnabled(false);
            animator.CrossFade(BossAnimIDs.Burrow, crossFadeDuration);
            _phase = Phase.Diving;
        }
    }

    private void UpdateDiving()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == BossAnimIDs.Burrow &&
            stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            agent.SetVisible(false);
            agent.RollNextAttack();
            _phase = Phase.Underground;
        }
    }

    private void UpdateUnderground()
    {
        var playerPos = _playerDetector.Player.position;
        var raw = new Vector3(playerPos.x, agent.transform.position.y, playerPos.z);
        var target = NavMesh.SamplePosition(raw, out var hit, _emergeSampleRadius, NavMesh.AllAreas)
            ? new Vector3(hit.position.x, agent.transform.position.y, hit.position.z)
            : raw;

        agent.transform.position = Vector3.MoveTowards(
            agent.transform.position, target, _undergroundSpeed * Time.deltaTime);

        if (Vector3.Distance(agent.transform.position, target) <= _arrivalThreshold)
        {
            _warningTimer = 0f;
            var pos = new Vector3(agent.transform.position.x, 0.25f, agent.transform.position.z);
            _activeWarning = Object.Instantiate(_warningPrefab, pos, Quaternion.identity);
            var col = _activeWarning.GetComponent<Collider>();
            _warningRadius = col != null ? col.bounds.extents.x : agent.ArrivalThreshold;
            _phase = Phase.Warning;
        }
    }

    private void UpdateWarning()
    {
        _warningTimer += Time.deltaTime;
        if (_warningTimer >= _warningDuration)
        {
            Object.Destroy(_activeWarning);
            _activeWarning = null;
            agent.SetVisible(true);
            _navMeshAgent.enabled = true;
            animator.CrossFade(BossAnimIDs.Emerge, crossFadeDuration);
            _phase = Phase.Emerging;
        }
    }

    private void UpdateEmerging()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!_emergeStarted)
        {
            if (stateInfo.shortNameHash == BossAnimIDs.Emerge)
                _emergeStarted = true;
            return;
        }

        if (!_emergeHitDealt)
        {
            _emergeHitDealt = true;
            var playerXZ = new Vector3(_playerDetector.Player.position.x, 0f, _playerDetector.Player.position.z);
            var bossXZ   = new Vector3(agent.transform.position.x, 0f, agent.transform.position.z);
            if (Vector3.Distance(playerXZ, bossXZ) <= _warningRadius)
                _playerDetector.PlayerHealth.TakeDamage(agent.MeleeDamage);
        }

        if (stateInfo.shortNameHash != BossAnimIDs.Emerge)
        {
            agent.SetColliderEnabled(true);
            IsComplete = true;
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            agent.SetColliderEnabled(true);
            IsComplete = true;
        }
    }

    public override void OnExit()
    {
        agent.SetVisible(true);
        agent.SetColliderEnabled(true);
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = true;
        IsComplete = false;
        if (_activeWarning != null)
        {
            Object.Destroy(_activeWarning);
            _activeWarning = null;
        }
    }
}
