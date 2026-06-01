using UnityEngine;
using UnityEngine.AI;

public class BossPhaseTransitionState : BaseState<BossController>
{
    private readonly NavMeshAgent _navMeshAgent;

    private bool _hasTriggeredPhase2;
    public bool IsComplete { get; private set; }

    public BossPhaseTransitionState(BossController agent, NavMeshAgent navMeshAgent, Animator animator)
        : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
    }

    public override void OnEnter()
    {
        IsComplete = false;
        _hasTriggeredPhase2 = false;
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.PhaseTransition, crossFadeDuration);
        agent.PlaySound(agent.PhaseTransitionSound);
    }

    public override void Update()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isTransitionPlaying = stateInfo.shortNameHash == BossAnimIDs.PhaseTransition;

        if (!isTransitionPlaying) return;

        if (!_hasTriggeredPhase2 && stateInfo.normalizedTime >= 0.5f)
        {
            _hasTriggeredPhase2 = true;
            agent.TransitionToPhase2();
        }

        if (stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
            IsComplete = true;
    }

    public override void OnExit()
    {
        _navMeshAgent.isStopped = false;
        IsComplete = false;
    }
}
