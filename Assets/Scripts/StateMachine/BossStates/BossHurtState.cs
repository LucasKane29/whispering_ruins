using UnityEngine;
using UnityEngine.AI;

public class BossHurtState : BaseState<BossController>
{
    private readonly NavMeshAgent _navMeshAgent;

    public BossHurtState(BossController agent, NavMeshAgent navMeshAgent, Animator animator)
        : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.Hurt, crossFadeDuration);
    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = false;
    }

    public void ReTrigger()
    {
        animator.CrossFade(BossAnimIDs.Hurt, crossFadeDuration);
    }
}
