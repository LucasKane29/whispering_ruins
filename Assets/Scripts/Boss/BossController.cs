using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class BossController : MonoBehaviour, IStatable
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private float _attackRange = 3f;
    [SerializeField] private float _mediumAttackRange = 6f;
    [SerializeField] private float _meleeDamage = 20f;
    [SerializeField] private float _hurtCooldown = 0.9f;
    [SerializeField] private float _aoeChance = 0.3f;
    [SerializeField] private float _rangedChance = 0.3f;
    [SerializeField] private float _undergroundSpeed = 8f;
    [SerializeField] private float _arrivalThreshold = 2f;
    [SerializeField] private GameObject _aoePrefab;
    [SerializeField] private Transform _aoeSpawnPoint;
    [SerializeField] private Health _health;
    [SerializeField] private PlayerDetector _playerDetector;
    [SerializeField] private Collider _collider;
    [SerializeField] private GameObject _burrowWarningPrefab;
    [SerializeField] private float _burrowWarningDuration = 2f;
    [SerializeField] private GameObject _aoeWarningPrefab;
    [SerializeField] private float _aoeWarningDuration = 1.5f;
    [SerializeField] private float _attackDelay = 2f;
    [SerializeField] private float _burrowDelay = 1f;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private float _projectileSpeed = 10f;
    [SerializeField] private float _rangedDamage = 15f;
    [SerializeField] private float _rangedAttackRange = 12f;
    [SerializeField] private float _aoeDamage = 25f;
    [SerializeField] private float _emergeSampleRadius = 5f;
    [SerializeField] private string _bossName = string.Empty;
    [SerializeField] private EventChannel _onBossKilled;
    [SerializeField] private PuzzleReward[] _rewards;
    [SerializeField] private float _deathLingerDuration = 5f;
    [SerializeField] private AudioClip _bossMusicClip;
    [SerializeField] private float _bossMusicFadeDuration = 1f;

    [Header("Sounds")]
    [SerializeField] private AudioClip _hurtSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _meleeAttackSound;
    [SerializeField] private AudioClip _mediumRangeAttackSound;
    [SerializeField] private AudioClip _rangedAttackSound;
    [SerializeField] private AudioClip _aoeAttackSound;
    [SerializeField] private AudioClip _phaseTransitionSound;
    [SerializeField] private AudioClip _burrowSound;
    [SerializeField] private AudioClip _emergeSound;
    [Range(0f, 1f)][SerializeField] private float _soundVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float _musicVolume = 1f;

    public event Action OnDeathAnimationComplete;
    public event Action OnBossActivated;
    public event Action OnAOEAnimationEvent;

    private StateMachine _stateMachine;
    private CountdownTimer _hurtTimer;


    private bool _isHurt;
    private bool _isDead;
    private bool _isActivated;
    private bool _phaseTransitionDone;

    public Health Health { get => _health; }
    public void SetColliderEnabled(bool enabled) => _collider.enabled = enabled;
    public GameObject BurrowWarningPrefab => _burrowWarningPrefab;
    public float BurrowWarningDuration => _burrowWarningDuration;
    public GameObject AoeWarningPrefab => _aoeWarningPrefab;
    public float AoeWarningDuration => _aoeWarningDuration;
    public float AttackDelay => _attackDelay;
    public float BurrowDelay => _burrowDelay;
    public GameObject ProjectilePrefab => _projectilePrefab;
    public Transform ProjectileSpawnPoint => _projectileSpawnPoint;
    public Transform AoeSpawnPoint => _aoeSpawnPoint;
    public float ProjectileSpeed => _projectileSpeed;
    public float RangedDamage => _rangedDamage;
    public float AoeDamage => _aoeDamage;
    public float RangedAttackRange => _rangedAttackRange;
    public string BossName => _bossName;

    public int CurrentPhase { get; private set; } = 1;
    public bool NextAttackIsAOE { get; private set; }
    public bool NextAttackIsRanged { get; private set; }
    public bool IsHurt { get => _isHurt; set => _isHurt = value; }
    public bool IsDead { get => _isDead; set => _isDead = value; }
    public bool IsActivated => _isActivated;
    public bool ShouldTransitionPhase =>
        CurrentPhase == 1 && !_phaseTransitionDone &&
        _health.CurrentHealth / _health.MaxHealth <= 0.5f;
    public float AttackRange => _attackRange;
    public float MediumAttackRange => _mediumAttackRange;
    public float MeleeDamage => _meleeDamage;
    public float UndergroundSpeed => _undergroundSpeed;
    public float ArrivalThreshold => _arrivalThreshold;
    public float EmergeSampleRadius => _emergeSampleRadius;
    public GameObject AoePrefab => _aoePrefab;
    public PlayerDetector PlayerDetector => _playerDetector;

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _hurtTimer = new CountdownTimer(_hurtCooldown);
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDeath -= HandleDeath;
        _health.OnDeath += HandleDeath;
        _hurtTimer.OnTimerStop += () => IsHurt = false;
    }

    private void OnDestroy()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDeath -= HandleDeath;
    }

    private void Start()
    {
        var idleState = new BossIdleState(this, _animator, _playerDetector);
        var phase1State = new BossPhase1State(this, _agent, _animator, _playerDetector);
        var phase2State = new BossPhase2State(this, _agent, _animator, _playerDetector);
        var phaseTransitionState = new BossPhaseTransitionState(this, _agent, _animator);
        var hurtState = new BossHurtState(this, _agent, _animator);
        var dieState = new BossDieState(this, _agent, _animator, _deathLingerDuration);

        Any(phaseTransitionState, new FunctionPredicate(() =>
            ShouldTransitionPhase &&
            _stateMachine.CurrentState is not BossPhaseTransitionState &&
            _stateMachine.CurrentState is not BossHurtState &&
            _stateMachine.CurrentState is not BossDieState));

        Any(hurtState, new FunctionPredicate(() =>
            IsHurt &&
            _stateMachine.CurrentState is not BossHurtState &&
            _stateMachine.CurrentState is not BossPhaseTransitionState &&
            _stateMachine.CurrentState is not BossDieState));

        Any(dieState, new FunctionPredicate(() =>
            IsDead && _stateMachine.CurrentState is not BossDieState));

        At(idleState, phase1State, new FunctionPredicate(() =>
            _isActivated && _playerDetector.CanDetectPlayer()));

        At(phaseTransitionState, phase2State, new FunctionPredicate(() =>
            phaseTransitionState.IsComplete));

        At(hurtState, phase1State, new FunctionPredicate(() =>
            !IsHurt && CurrentPhase == 1));

        At(hurtState, phase2State, new FunctionPredicate(() =>
            !IsHurt && CurrentPhase == 2));

        _stateMachine.SetState(idleState);
    }

    public void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        _stateMachine.Update();
        _hurtTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate() => _stateMachine.FixedUpdate();

    private void HandleHealthChanged(float percentage)
    {
        if (_health.CurrentHealth <= 0f) return;

        if (_hurtTimer.IsRunning)
        {
            _hurtTimer.Start();
            return;
        }
        IsHurt = true;
        _hurtTimer.Start();
        PlaySound(_hurtSound);
    }

    private void HandleDeath()
    {
        IsDead = true;
        PlaySound(_deathSound);
    }

    public void ActivateBoss()
    {
        _isActivated = true;
        OnBossActivated?.Invoke();
        if (_bossMusicClip != null)
            IServiceLocator.Instance.GetService<ISoundService>()?.PlayMusic(_bossMusicClip, _bossMusicFadeDuration, true, _musicVolume);
    }

    public void RollNextAttack()
    {
        NextAttackIsAOE = false;
        NextAttackIsRanged = false;
        if (CurrentPhase != 2) return;

        float roll = UnityEngine.Random.value;
        if (roll < _aoeChance)
            NextAttackIsAOE = true;
        else if (roll < _aoeChance + _rangedChance)
            NextAttackIsRanged = true;
    }

    public void TransitionToPhase2()
    {
        CurrentPhase = 2;
        _phaseTransitionDone = true;
    }

    public void SetVisible(bool visible)
    {
        foreach (var r in _renderers)
            r.enabled = visible;
    }

    public void NotifyDeathAnimationComplete()
    {
        OnDeathAnimationComplete?.Invoke();
        _onBossKilled?.Invoke(new Empty());
        foreach (var reward in _rewards)
            reward?.Give();
    }

    public void AOESpawnEvent() => OnAOEAnimationEvent?.Invoke();

    public AudioClip BurrowSound           => _burrowSound;
    public AudioClip EmergeSound           => _emergeSound;
    public AudioClip MeleeAttackSound      => _meleeAttackSound;
    public AudioClip MediumRangeAttackSound => _mediumRangeAttackSound;
    public AudioClip RangedAttackSound     => _rangedAttackSound;
    public AudioClip AoeAttackSound        => _aoeAttackSound;
    public AudioClip PhaseTransitionSound  => _phaseTransitionSound;

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        IServiceLocator.Instance.GetService<ISoundService>()
            ?.PlayOneShot(clip, transform.position, _soundVolume);
    }
}
