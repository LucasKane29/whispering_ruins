using UnityEngine;
using UnityEngine.AI;

public class BossPhase2State : BossBaseState
{
    private readonly StateMachine _innerFsm = new();
    private readonly NavMeshAgent _navMeshAgent;
    private readonly BossBurrowState _burrowState;
    private readonly BossMeleeAttackState _meleeState;
    private readonly BossRangedAttackState _rangedState;
    private readonly BossAOEAttackState _aoeState;
    private IState _resumeState;

    public BossPhase2State(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator, playerDetector)
    {
        _navMeshAgent = navMeshAgent;
        _burrowState  = new BossBurrowState(agent, navMeshAgent, animator, playerDetector);
        _meleeState   = new BossMeleeAttackState(agent, navMeshAgent, animator, playerDetector);
        _rangedState  = new BossRangedAttackState(agent, navMeshAgent, animator, playerDetector);
        _aoeState     = new BossAOEAttackState(agent, navMeshAgent, animator, playerDetector);

        _innerFsm.AddTransition(_burrowState, _meleeState,
            new FunctionPredicate(() => _burrowState.IsComplete
                && playerDetector.CanAttackPlayer(agent.AttackRange)));

        _innerFsm.AddTransition(_burrowState, _aoeState,
            new FunctionPredicate(() => _burrowState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.AttackRange)
                && playerDetector.CanAttackPlayer(agent.MediumAttackRange)));

        _innerFsm.AddTransition(_burrowState, _rangedState,
            new FunctionPredicate(() => _burrowState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.MediumAttackRange)
                && playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_burrowState, _burrowState,
            new FunctionPredicate(() => _burrowState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_meleeState, _aoeState,
            new FunctionPredicate(() => _meleeState.IsComplete
                && playerDetector.CanAttackPlayer(agent.MediumAttackRange)));

        _innerFsm.AddTransition(_meleeState, _rangedState,
            new FunctionPredicate(() => _meleeState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.MediumAttackRange)
                && playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_meleeState, _burrowState,
            new FunctionPredicate(() => _meleeState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_aoeState, _meleeState,
            new FunctionPredicate(() => _aoeState.IsComplete
                && playerDetector.CanAttackPlayer(agent.AttackRange)));

        _innerFsm.AddTransition(_aoeState, _rangedState,
            new FunctionPredicate(() => _aoeState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.MediumAttackRange)
                && playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_aoeState, _burrowState,
            new FunctionPredicate(() => _aoeState.IsComplete
                && !playerDetector.CanAttackPlayer(agent.RangedAttackRange)));

        _innerFsm.AddTransition(_rangedState, _burrowState,
            new FunctionPredicate(() => _rangedState.IsComplete));
    }

    public override void OnEnter()
    {
        if (_resumeState == null)
        {
            _innerFsm.SetState(_burrowState);
            return;
        }

        if (_resumeState == _burrowState)
        {
            if (playerDetector.CanAttackPlayer(agent.AttackRange))
                _innerFsm.SetState(_meleeState);
            else if (playerDetector.CanAttackPlayer(agent.MediumAttackRange))
                _innerFsm.SetState(_aoeState);
            else if (playerDetector.CanAttackPlayer(agent.RangedAttackRange))
                _innerFsm.SetState(_rangedState);
            else
                _innerFsm.SetState(_burrowState);
        }
        else
        {
            _innerFsm.SetState(_resumeState);
        }

        _resumeState = null;
    }

    public override void Update()       => _innerFsm.Update();
    public override void FixedUpdate()  => _innerFsm.FixedUpdate();

    public override void OnExit()
    {
        _resumeState = _innerFsm.CurrentState;
        agent.SetVisible(true);
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = true;
    }
}
