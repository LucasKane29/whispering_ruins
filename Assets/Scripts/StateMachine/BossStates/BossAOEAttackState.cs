using UnityEngine;
using UnityEngine.AI;

public class BossAOEAttackState : BossBaseState
{
    private enum Phase { PreAttack, Attacking, PostDelay }

    private readonly NavMeshAgent _navMeshAgent;
    private readonly GameObject _warningPrefab;
    private readonly float _warningDuration;

    private Phase _phase;
    private float _phaseTimer;
    private bool _isActive;
    private GameObject _activeWarning;

    public bool IsComplete { get; private set; }

    public BossAOEAttackState(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator, playerDetector)
    {
        _navMeshAgent    = navMeshAgent;
        _warningPrefab   = agent.AoeWarningPrefab;
        _warningDuration = agent.AoeWarningDuration;

        agent.OnAOEAnimationEvent += () => { if (_isActive) { SpawnAOE(); agent.PlaySound(agent.AoeAttackSound); } };
    }

    public override void OnEnter()
    {
        IsComplete = false;
        _isActive  = true;
        _navMeshAgent.isStopped = true;
        EnterPreAttack();
    }

    public override void Update()
    {
        FacePlayer();
        TrackPlayerWithSpawnPoint();

        switch (_phase)
        {
            case Phase.PreAttack:  UpdatePreAttack();  break;
            case Phase.Attacking:  UpdateAttacking();  break;
            case Phase.PostDelay:  UpdatePostDelay();  break;
        }
    }

    public override void OnExit()
    {
        _isActive = false;
        _navMeshAgent.isStopped = false;
        IsComplete = false;
        DestroyWarning();
    }

    private void EnterPreAttack()
    {
        _phase      = Phase.PreAttack;
        _phaseTimer = _warningDuration;
        animator.CrossFade(BossAnimIDs.Idle, crossFadeDuration);
        SpawnWarning();
    }

    private void UpdatePreAttack()
    {
        _phaseTimer -= Time.deltaTime;
        if (_phaseTimer > 0f) return;

        DestroyWarning();
        _phase = Phase.Attacking;
        animator.CrossFade(BossAnimIDs.AOEAttack, crossFadeDuration);
    }

    private void UpdateAttacking()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != BossAnimIDs.AOEAttack) return;

        if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            _phase      = Phase.PostDelay;
            _phaseTimer = agent.AttackDelay;
            animator.CrossFade(BossAnimIDs.Idle, crossFadeDuration);
        }
    }

    private void UpdatePostDelay()
    {
        _phaseTimer -= Time.deltaTime;
        if (_phaseTimer > 0f) return;

        if (playerDetector.CanAttackPlayer(agent.MediumAttackRange)
            && !playerDetector.CanAttackPlayer(agent.AttackRange))
        {
            EnterPreAttack();
        }
        else
        {
            IsComplete = true;
        }
    }

    private void SpawnWarning()
    {
        if (_warningPrefab == null) return;
        _activeWarning = Object.Instantiate(
            _warningPrefab,
            agent.transform.position,
            agent.transform.rotation,
            agent.transform);
    }

    private void TrackPlayerWithSpawnPoint()
    {
        if (agent.AoeSpawnPoint == null) return;
        var dir = (playerDetector.Player.position - agent.AoeSpawnPoint.position).normalized;
        if (dir != Vector3.zero)
            agent.AoeSpawnPoint.rotation = Quaternion.LookRotation(dir);
    }

    private void SpawnAOE()
    {
        if (agent.AoePrefab == null) return;
        var pos = agent.AoeSpawnPoint != null
            ? agent.AoeSpawnPoint.position
            : agent.transform.position + Vector3.forward;
        var dir = (playerDetector.Player.position - pos).normalized;
        var rot = dir != Vector3.zero
            ? Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 90f, 0f)
            : agent.transform.rotation;
        var go = Object.Instantiate(agent.AoePrefab, pos, rot);
        go.GetComponent<BossAOEEffect>()?.Init(agent.AoeDamage);
    }

    private void DestroyWarning()
    {
        if (_activeWarning == null) return;
        Object.Destroy(_activeWarning);
        _activeWarning = null;
    }
}
