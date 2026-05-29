using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState: BaseState<EnemyController>
{
    private Transform _player;
    private NavMeshAgent _navMeshAgent;
    private float _stoppingDistance;
    private PlayerDetector _playerDetector;

    public override void OnEnter()
    {
        _navMeshAgent.stoppingDistance = _stoppingDistance;
        _navMeshAgent.updateRotation = false;
        animator.CrossFade(EnemyAnimIDs.Run, crossFadeDuration);
    }

    public EnemyChaseState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator, PlayerDetector playerDetector, float stoppingDistance) : base(agent, animator)
    {
        _playerDetector = playerDetector;
        _navMeshAgent = navMeshAgent;
        _stoppingDistance = stoppingDistance;
    }

    public override void OnExit()
    {
        _navMeshAgent.updateRotation = true;
    }

    public override void Update()
    {
        _player = _playerDetector.Player;
        _navMeshAgent.SetDestination(_player.position);

        var directionToPlayer = (_player.position - agent.transform.position).normalized;
        directionToPlayer.y = 0f;
        if (directionToPlayer != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(directionToPlayer);
            agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}