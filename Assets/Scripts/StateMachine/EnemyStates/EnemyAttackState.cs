using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackState : BaseState<EnemyController>
{
    private Transform _player;
    private NavMeshAgent _navMeshAgent;
    private PlayerDetector _playerDetector;

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = true;
        _navMeshAgent.avoidancePriority = 0;
        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        animator.CrossFade(EnemyAnimIDs.Attack, crossFadeDuration);

    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        _navMeshAgent.avoidancePriority = 50;
    }

    public EnemyAttackState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator, PlayerDetector playerDetector) : base(agent, animator)
    {
        _playerDetector = playerDetector;
        _navMeshAgent = navMeshAgent;
    }

    public override void Update()
    {
        _player = _playerDetector.Player;
        var directionToPlayer = (_player.position - agent.transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(directionToPlayer);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        agent.Attack();
    }
}
