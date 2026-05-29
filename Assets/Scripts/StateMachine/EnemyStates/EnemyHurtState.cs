using UnityEngine;
using UnityEngine.AI;

public class EnemyHurtState : BaseState<EnemyController>
{
    private NavMeshAgent _navMeshAgent;

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = true;
        animator.CrossFade(EnemyAnimIDs.Hurt, crossFadeDuration);

    }

    public EnemyHurtState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator) : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = false;
    }

    public void ReTrigger()
    {
        animator.CrossFade(EnemyAnimIDs.Hurt, crossFadeDuration);
    }
}
