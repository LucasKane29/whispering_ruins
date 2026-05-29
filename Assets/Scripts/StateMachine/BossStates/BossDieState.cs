using UnityEngine;
using UnityEngine.AI;

public class BossDieState : BaseState<BossController>
{
    private readonly NavMeshAgent _navMeshAgent;

    public BossDieState(BossController agent, NavMeshAgent navMeshAgent, Animator animator)
        : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.Die, crossFadeDuration);
    }

    public override void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isDiePlaying = stateInfo.shortNameHash == BossAnimIDs.Die;

        if (isDiePlaying && stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            agent.NotifyDeathAnimationComplete();
            agent.gameObject.SetActive(false);
        }
    }
}
