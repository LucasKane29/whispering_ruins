using UnityEngine;
using UnityEngine.AI;

public class BossDieState : BaseState<BossController>
{
    private readonly NavMeshAgent _navMeshAgent;
    private readonly CountdownTimer _lingerTimer;
    private bool _animationFinished;

    public BossDieState(BossController agent, NavMeshAgent navMeshAgent, Animator animator, float lingerDuration)
        : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
        _lingerTimer = new CountdownTimer(lingerDuration);
        _lingerTimer.OnTimerStop += () =>
        {
            agent.NotifyDeathAnimationComplete();
            agent.gameObject.SetActive(false);
        };
    }

    public override void OnEnter()
    {
        _animationFinished = false;
        _navMeshAgent.isStopped = true;
        animator.CrossFade(BossAnimIDs.Die, crossFadeDuration);
    }

    public override void Update()
    {
        if (!_animationFinished)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            bool isDiePlaying = stateInfo.shortNameHash == BossAnimIDs.Die;

            if (isDiePlaying && stateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
            {
                _animationFinished = true;
                _lingerTimer.Start();
            }
        }

        _lingerTimer.Tick(Time.deltaTime);
    }
}
