using UnityEngine;
using UnityEngine.AI;

public class BossPhase1State : BaseState<BossController>
{
    private readonly StateMachine _innerFsm = new();
    private readonly NavMeshAgent _navMeshAgent;
    private readonly PlayerDetector _playerDetector;
    private readonly BossBurrowState _burrowState;
    private readonly BossMeleeAttackState _meleeState;
    private readonly BossMediumRangeAttackState _mediumState;
    private IState _resumeState;

    public BossPhase1State(BossController agent, NavMeshAgent navMeshAgent, Animator animator,
        PlayerDetector playerDetector) : base(agent, animator)
    {
        _navMeshAgent = navMeshAgent;
        _playerDetector = playerDetector;
        _burrowState = new BossBurrowState(agent, navMeshAgent, animator, playerDetector);
        _meleeState = new BossMeleeAttackState(agent, navMeshAgent, animator, playerDetector);
        _mediumState = new BossMediumRangeAttackState(agent, navMeshAgent, animator, playerDetector);

        _innerFsm.AddTransition(_burrowState, _meleeState,
            new FunctionPredicate(() => _burrowState.IsComplete && playerDetector.CanAttackPlayer(agent.AttackRange)));
        _innerFsm.AddTransition(_burrowState, _mediumState,
            new FunctionPredicate(() => _burrowState.IsComplete && !playerDetector.CanAttackPlayer(agent.AttackRange)));

        _innerFsm.AddTransition(_meleeState, _mediumState,
            new FunctionPredicate(() => _meleeState.IsComplete && playerDetector.CanAttackPlayer(agent.MediumAttackRange)));
        _innerFsm.AddTransition(_meleeState, _burrowState,
            new FunctionPredicate(() => _meleeState.IsComplete && !playerDetector.CanAttackPlayer(agent.MediumAttackRange)));

        _innerFsm.AddTransition(_mediumState, _burrowState,
            new FunctionPredicate(() => _mediumState.IsComplete));
    }

    public override void OnEnter()
    {
        if (_resumeState == null)
        {
            _innerFsm.SetState(_burrowState);
        }
        else if (_resumeState == _burrowState)
        {
            _innerFsm.SetState(_playerDetector.CanAttackPlayer(agent.AttackRange) ? (IState)_meleeState : _mediumState);
        }
        else
        {
            _innerFsm.SetState(_resumeState);
        }
        _resumeState = null;
    }

    public override void Update() => _innerFsm.Update();
    public override void FixedUpdate() => _innerFsm.FixedUpdate();

    public override void OnExit()
    {
        _resumeState = _innerFsm.CurrentState;
        agent.SetVisible(true);
        _navMeshAgent.enabled = true;
        _navMeshAgent.isStopped = true;
    }
}
