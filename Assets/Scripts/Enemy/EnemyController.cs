using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class EnemyController : MonoBehaviour, IStatable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _wanderRadius = 5f;
    [SerializeField] private float _attackCooldown = 1f;
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackDamage = 10f;
    [SerializeField] private float _hurtCooldown = 0.85f;
    [SerializeField] private int _deathScore = 50;
    [SerializeField] private IntEventChannel _channel;
    public event Action OnDeathAnimationComplete;

    private StateMachine _stateMachine;

    private CountdownTimer _attackTimer;
    private CountdownTimer _hurtTimer;

    private Health _health;
    private bool _isHurt = false;
    private bool _isDead = false;
    private PlayerDetector _playerDetector;
    public bool IsHurt
    {
        get => _isHurt;
        set
        {
            _isHurt = value;
        }
    }

    public bool IsDead
    {
        get => _isDead;
        set
        {
            _isDead = value;
        }
    }

    public Health Health => _health;

    private void Awake()
    {
        _playerDetector = GetComponent<PlayerDetector>();
        _stateMachine = new StateMachine();
        _health = GetComponent<Health>();
        _attackTimer = new CountdownTimer(_attackCooldown);
        _hurtTimer = new CountdownTimer(_hurtCooldown);
        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDeath += HandleDeath;

        _hurtTimer.OnTimerStop += () => IsHurt = false;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
    }

    private void HandleHealthChanged(float percentage)
    {
        if(_health.CurrentHealth > 0f)
        {
            IsHurt = true;
            _hurtTimer.Start();

            if (_stateMachine.CurrentState is EnemyHurtState hurtState)
                hurtState.ReTrigger();
        }
    }

    private void HandleDeath()
    {
        IsDead = true;
    }

    private void Start()
    {
        var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius);
        var chaseState = new EnemyChaseState(this, _agent, _animator, _playerDetector, _attackRange);
        var attackState = new EnemyAttackState(this, _agent, _animator, _playerDetector);
        var hurtState = new EnemyHurtState(this, _agent, _animator);
        var dieState = new EnemyDieState(this, _agent, _animator, _channel, _deathScore);

        Any(hurtState, new FunctionPredicate(() => IsHurt && _stateMachine.CurrentState is not EnemyHurtState));
        Any(dieState, new FunctionPredicate(() => IsDead && _stateMachine.CurrentState is not EnemyDieState));
        At(wanderState, chaseState, new FunctionPredicate(() => _playerDetector.CanDetectPlayer()));
        At(chaseState, wanderState, new FunctionPredicate(() => !_playerDetector.CanDetectPlayer()));
        At(chaseState, attackState, new FunctionPredicate(() => _playerDetector.CanAttackPlayer(_attackRange)));
        At(attackState, chaseState, new FunctionPredicate(() => !_playerDetector.CanAttackPlayer(_attackRange)));
        At(hurtState, chaseState, new FunctionPredicate(() => !IsHurt && _playerDetector.CanDetectPlayer()));
        At(hurtState, wanderState, new FunctionPredicate(() => !IsHurt && !_playerDetector.CanDetectPlayer()));
        At(hurtState, attackState, new FunctionPredicate(() => !IsHurt && _playerDetector.CanAttackPlayer(_attackRange)));

        _stateMachine.SetState(wanderState);
    }

    public void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        _stateMachine.Update();
        _attackTimer.Tick(Time.deltaTime);
        _hurtTimer.Tick(Time.deltaTime);
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
        _playerDetector.PlayerHealth.TakeDamage(_attackDamage);
    }

    public void NotifyDeathAnimationComplete()
    {
        OnDeathAnimationComplete?.Invoke();
    }
}
