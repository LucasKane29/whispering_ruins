using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class EnemyController : MonoBehaviour, IStatable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private PlayerDetector _playerDetector;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _attackRange = 2f;

    private StateMachine _stateMachine;

    private CountdownTimer _attackTimer;

    private void Awake()
    {
        _stateMachine = new StateMachine();
    }

    private void Start()
    {
        _attackTimer = new CountdownTimer(_attackCooldown);

        var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius);
        var chaseState = new EnemyChaseState(this, _agent, _animator, _playerDetector.Player);
        var attackState = new EnemyAttackState(this, _agent, _animator, _playerDetector.Player);

        At(wanderState, chaseState, new FunctionPredicate(() => _playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FunctionPredicate(() => !_playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FunctionPredicate(() => _playerDetector.CanAttackPlayer(_attackRange)));
        At(attackState, chaseState, new FunctionPredicate(() => !_playerDetector.CanAttackPlayer(_attackRange)));

        _stateMachine.SetState(wanderState);
    }

    public void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        _stateMachine.Update();
        _attackTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    public void Attack()
    {
        if (_attackTimer.IsRunning) 
            return;
        _attackTimer.Start();
        Debug.Log("Attacking!");
    }
}
