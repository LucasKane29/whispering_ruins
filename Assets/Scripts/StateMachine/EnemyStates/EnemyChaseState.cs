using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState: BaseState<EnemyController>
{
    private Transform _player;
    private NavMeshAgent _navMeshAgent;

    public override void OnEnter()
    {
        Debug.Log("Entering Chase State");
        animator.CrossFade(EnemyAnimIDs.Run, crossFadeDuration);

    }

    public EnemyChaseState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator, Transform player) : base(agent, animator)
    {
        _player = player;
        _navMeshAgent = navMeshAgent;
    }

    public override void Update()
    {
        _navMeshAgent.SetDestination(_player.position);
    }
}

public class EnemyAttackState : BaseState<EnemyController>
{
    private Transform _player;
    private NavMeshAgent _navMeshAgent;

    public override void OnEnter()
    {
        Debug.Log("Entering Attack State");
        animator.CrossFade(EnemyAnimIDs.Attack, crossFadeDuration);

    }

    public EnemyAttackState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator, Transform player) : base(agent, animator)
    {
        _player = player;
        _navMeshAgent = navMeshAgent;
    }

    public override void Update()
    {
        _navMeshAgent.SetDestination(_player.position);
        agent.Attack();
    }
}