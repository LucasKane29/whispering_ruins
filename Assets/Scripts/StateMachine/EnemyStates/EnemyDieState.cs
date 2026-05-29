using UnityEngine;
using UnityEngine.AI;

public class EnemyDieState : BaseState<EnemyController>
{
    private NavMeshAgent _navMeshAgent;

    private IntEventChannel _channel;
    private int _scoreValue;

    public override void OnEnter()
    {
        _navMeshAgent.isStopped = true;
        animator.CrossFade(EnemyAnimIDs.Die, crossFadeDuration);
    }

    public EnemyDieState(EnemyController agent, NavMeshAgent navMeshAgent, Animator animator, IntEventChannel channel, int deathScore) : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
        _channel = channel;
        _scoreValue = deathScore;
    }

    public override void Update()
    {
        var animStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isDiePlaying = animStateInfo.shortNameHash == EnemyAnimIDs.Die;
        if (isDiePlaying && animStateInfo.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            agent.NotifyDeathAnimationComplete();
            agent.gameObject.SetActive(false);
            if(_channel != null) 
               _channel.Invoke(_scoreValue);
        }
    }

    public override void OnExit() { }
}
