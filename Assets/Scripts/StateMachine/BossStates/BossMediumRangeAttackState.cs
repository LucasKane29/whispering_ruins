using UnityEngine;
using UnityEngine.AI;

public class BossMediumRangeAttackState : BossBaseState
{
    private readonly NavMeshAgent _navMeshAgent;

    private bool _hasDealtDamage;
    private bool _isWaiting;
    private float _delayTimer;
    public bool IsComplete { get; private set; }

    public BossMediumRangeAttackState(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator, playerDetector)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnEnter()
    {
        IsComplete = false;
        _hasDealtDamage = false;
        _isWaiting = false;
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.MediumRangeAttack, crossFadeDuration);
    }

    public override void Update()
    {
        FacePlayer();

        if (_isWaiting)
        {
            _delayTimer -= Time.deltaTime;
            if (_delayTimer <= 0f)
            {
                if (playerDetector.CanAttackPlayer(agent.AttackRange))
                {
                    IsComplete = true; // player too close — escalate to melee
                }
                else if (playerDetector.CanAttackPlayer(agent.MediumAttackRange))
                {
                    _isWaiting = false;
                    _hasDealtDamage = false;
                    animator.CrossFade(BossAnimIDs.MediumRangeAttack, crossFadeDuration);
                }
                else
                {
                    IsComplete = true;
                }
            }
            return;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != BossAnimIDs.MediumRangeAttack) return;

        if (!_hasDealtDamage && stateInfo.normalizedTime >= 0.5f)
        {
            _hasDealtDamage = true;
            playerDetector.PlayerHealth.TakeDamage(agent.MeleeDamage);
            agent.PlaySound(agent.MediumRangeAttackSound);
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
}
