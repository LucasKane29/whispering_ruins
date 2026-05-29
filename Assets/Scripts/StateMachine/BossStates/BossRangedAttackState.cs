using UnityEngine;
using UnityEngine.AI;

public class BossRangedAttackState : BossBaseState
{
    private readonly NavMeshAgent _navMeshAgent;

    private bool _hasSpawnedProjectile;
    private bool _isWaiting;
    private float _delayTimer;
    public bool IsComplete { get; private set; }

    public BossRangedAttackState(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator, playerDetector)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnEnter()
    {
        IsComplete = false;
        _hasSpawnedProjectile = false;
        _isWaiting = false;
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.RangedAttack, crossFadeDuration);
    }

    public override void Update()
    {
        FacePlayer();

        if (_isWaiting)
        {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f)
            {
                if (playerDetector.CanAttackPlayer(agent.RangedAttackRange)
                    && !playerDetector.CanAttackPlayer(agent.MediumAttackRange))
                {
                    _isWaiting = false;
                    _hasSpawnedProjectile = false;
                    animator.CrossFade(BossAnimIDs.RangedAttack, crossFadeDuration);
                }
                else
                {
                    IsComplete = true;
                }
            }
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != BossAnimIDs.RangedAttack) return;

        if (!_hasSpawnedProjectile && stateInfo.normalizedTime >= 0.4f)
        {
            _hasSpawnedProjectile = true;
            SpawnProjectile();
        }

        if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            _isWaiting = true;
            _delayTimer = agent.AttackDelay;
            animator.CrossFade(BossAnimIDs.Idle, crossFadeDuration);
        }
    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = false;
        _isWaiting = false;
        IsComplete = false;
    }

    private void SpawnProjectile()
    {
        if (agent.ProjectilePrefab == null) return;
        var spawnPos = agent.ProjectileSpawnPoint != null
            ? agent.ProjectileSpawnPoint.position
            : agent.transform.position + Vector3.up;
        var targetPos = playerDetector.Player.position + Vector3.up * 0.5f;
        var direction = (targetPos - spawnPos).normalized;
        var go = Object.Instantiate(agent.ProjectilePrefab, spawnPos, Quaternion.LookRotation(direction));
        go.GetComponent<BossProjectile>()?.Init(agent.ProjectileSpeed, agent.RangedDamage, direction);
    }
}
