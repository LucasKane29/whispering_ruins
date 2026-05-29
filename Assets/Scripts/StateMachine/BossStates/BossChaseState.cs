using UnityEngine;
using UnityEngine.AI;

public class BossChaseState : BaseState<BossController>
{
    private readonly NavMeshAgent _navMeshAgent;
    private readonly PlayerDetector _playerDetector;
    private readonly float _stoppingDistance;

    public BossChaseState(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector, float stoppingDistance) : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
        _playerDetector = playerDetector;
        _stoppingDistance = stoppingDistance;
    }

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.stoppingDistance = _stoppingDistance;
        _navMeshAgent.updateRotation = false;
        animator.CrossFade(BossAnimIDs.Run, crossFadeDuration);
        agent.RollNextAttack();
    }

    public override void Update()
    {
        _navMeshAgent.SetDestination(_playerDetector.Player.position);

        var directionToPlayer = (_playerDetector.Player.position - agent.transform.position).normalized;
        directionToPlayer.y = 0f;
        if (directionToPlayer != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(directionToPlayer);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.updateRotation = true;
    }
}
