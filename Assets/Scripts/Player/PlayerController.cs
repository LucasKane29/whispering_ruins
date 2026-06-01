using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IStatable, IPlayerService
{
    [Header("Гравець")]
    [Tooltip("Швидкість ходи персонажу у м/с")]
    [SerializeField] private float _moveSpeed = 2.0f;

    [Tooltip("Швидкість бігу персонажу у м/с")]
    [SerializeField] private float _sprintSpeed = 5.335f;

    [Tooltip("Наскільки швидко персонаж змінює напрямок")]
    [Range(0.0f, 0.3f)]
    [SerializeField] private float _rotationSmoothTime = 0.12f;

    [Tooltip("Прискорення та сповільнення")]
    [SerializeField] private float _speedChangeRate = 10.0f;

    [Header("Серцебиття")]
    [SerializeField] private AudioClip _heartbeatClip;
    [Range(0f, 1f)][SerializeField] private float _heartbeatVolume = 0.8f;
    [SerializeField] private float _lowHealthThreshold = 0.25f;

    [Header("Звуки")]
    [SerializeField] private AudioClip _landingAudioClip;
    [SerializeField] private AudioClip[] _footstepAudioClips;
    [SerializeField] private AudioClip[] _swordAttackAudioClips;
    [Range(0, 1)][SerializeField] private float _footstepAudioVolume = 0.5f;
    [Range(0, 1)][SerializeField] private float _swordAttackAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("Висота стрибку")]
    [SerializeField] private float _jumpHeight = 1.2f;

    [Tooltip("Персонаж використовує власне значення гравітації. Значення за замовчуванням -9.81f")]
    [SerializeField] private float _gravity = -15.0f;

    [Space(10)]
    [Tooltip("Час затримки перед стрибком")]
    [SerializeField] private float _jumpTimeout = 0.50f;

    [Tooltip("Час, необхідний для переходу в стан падіння. Корисно для спуску по сходах")]
    [SerializeField] private float _fallTimeout = 0.15f;

    [Header("Приземлення персонажу")]
    [Tooltip("Флаг показує чи на землі персонаж")]
    private bool _grounded = true;

    [Tooltip("Корисно для грубих поверхонь")]
    private float _groundedOffset = -0.14f;

    [Tooltip("Радіус перевірки на дотик до землі. Повинен відповідати радіусу CharacterController")]
    private float _groundedRadius = 0.28f;

    [Tooltip("Які шари персонаж використовує як землю")]
    [SerializeField] private LayerMask _groundLayers;

    [Header("Cinemachine")]
    [Tooltip("Ціль для слідування, встановлена в Cinemachine Virtual Camera, за якою камера буде слідувати")]
    [SerializeField] private GameObject _cinemachineCameraTarget;

    [Header("Налаштування атаки")]
    [Tooltip("Обмеження у часі через яке можна зробити наступну атаку")]
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private float _attackRange = 1.0f;
    [SerializeField] private float _attackDamage = 10.0f;

    [Header("Налаштування витрат стаміни")]
    [SerializeField] private float _attackStaminaCost = 10f;
    [SerializeField] private float _sprintStaminaCost = 2f;
    [SerializeField] private float _jumpStaminaCost = 5f;
    [SerializeField] private float _hurtDuration = 0.5f;
    [SerializeField] private float _danceDuration = 5f;
    [SerializeField] private float _deathLingerDuration = 2f;
    [SerializeField] private EventChannel _talkedToNPCChannel;

    private const float _terminalVelocity = 53.0f;
    private float _verticalVelocity;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private Animator _animator;
    private CharacterController _controller;
    private PlayerInputs _input;
    private GameObject _mainCamera;
    private bool _hasAnimator;
    private CountdownTimer _attackCooldownTimer;

    private StateMachine _stateMachine;


    public bool IsHurt { get; set; }
    public bool IsDead { get; private set; }
    public bool IsDancing { get; set; }

    public bool Grounded => _grounded;
    public float JumpHeight => _jumpHeight;
    public float Gravity => _gravity;
    public float JumpTimeout => _jumpTimeout;
    public float FallTimeout => _fallTimeout;
    public float Speed => _speed;
    public float AnimationBlend => _animationBlend;
    public float TargetRotation => _targetRotation;
    public float RotationVelocity => _rotationVelocity;
    public Animator Animator => _animator;
    public CharacterController Controller => _controller;
    public PlayerInputs Input => _input;
    public GameObject MainCamera => _mainCamera;
    public bool HasAnimator => _hasAnimator;
    public static float TerminalVelocity => _terminalVelocity;

    private Health _health;
    private AudioSource _heartbeatSource;

    private Stamina _stamina;
    private CameraService _cameraService;

    private bool _inputBlocked;
    private InventoryUIService _inventoryUIService;
    public float JumpTimeoutDelta
    {
        get => _jumpTimeoutDelta;
        set => _jumpTimeoutDelta = value;
    }
    public float VerticalVelocity
    {
        get => _verticalVelocity;
        set => _verticalVelocity = value;
    }

    public float FallTimeoutDelta
    {
        get => _fallTimeoutDelta;
        set => _fallTimeoutDelta = value;
    }

    public Transform Transform => _controller.transform;

    public Health Health => _health;
    public Stamina Stamina => _stamina;

    private void OnDisable()
    {
        DialogueManager.Instance.OnDialogueStarted -= BlockInput;
        DialogueManager.Instance.OnDialogueEnded -= UnblockInput;
        if (_talkedToNPCChannel != null)
            _talkedToNPCChannel.OnEventRaised -= OnTalkedToNPC;
    }

    private void BlockInput()
    {
        _inputBlocked = true;
        _input.move = Vector2.zero;
        _input.attack = false;
        _input.look = Vector2.zero;
        _input.sprint = false;
        _input.jump = false;
        _speed = 0f;
        _input.inventory = false;
    }

    private void UnblockInput()
    {
        StartCoroutine(UnblockNextFrame());
    }

    private void OnDestroy()
    {
        IServiceLocator.Instance.TryUnregisterService<IPlayerService, PlayerController>(this);
        if (_inventoryUIService != null)
        {
            _inventoryUIService.OnInventoryOpened -= BlockInput;
            _inventoryUIService.OnInventoryClosed -= UnblockInput;
        }
        var pauseService = IServiceLocator.Instance.GetService<IPauseService>();
        if (pauseService != null)
        {
            pauseService.OnPaused -= BlockInput;
            pauseService.OnResumed -= UnblockInput;
        }
    }

    void Awake() 
    { 
        if (IServiceLocator.Instance != null)
        {
            IServiceLocator.Instance.TryRegisterService<IPlayerService, PlayerController>(this);
        }
    }
      

    private void Start()
    {
        DialogueManager.Instance.OnDialogueStarted += BlockInput;
        DialogueManager.Instance.OnDialogueEnded += UnblockInput;
        _inventoryUIService = IServiceLocator.Instance.GetService<IInventoryUIService>() as InventoryUIService;
        if (_inventoryUIService != null)
        {
            _inventoryUIService.OnInventoryOpened += BlockInput;
            _inventoryUIService.OnInventoryClosed += UnblockInput;
        }

        var pauseService = IServiceLocator.Instance.GetService<IPauseService>();
        if (pauseService != null)
        {
            pauseService.OnPaused += BlockInput;
            pauseService.OnResumed += UnblockInput;
        }


        _attackCooldownTimer = new CountdownTimer(_attackCooldown);
        _attackCooldownTimer.OnTimerStop += () => _input.attack = false;
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        if (_health != null)
        {
            _health.OnDamaged += () => IsHurt = true;
            _health.OnDeath   += () => IsDead = true;
            _health.OnHealthChanged += HandleHeartbeat;
            _health.OnDeath         += StopHeartbeat;
        }

        _heartbeatSource = gameObject.AddComponent<AudioSource>();
        _heartbeatSource.clip = _heartbeatClip;
        _heartbeatSource.loop = true;
        _heartbeatSource.spatialBlend = 0f;
        _heartbeatSource.volume = _heartbeatVolume;
        _heartbeatSource.playOnAwake = false;
        if (_talkedToNPCChannel != null)
            _talkedToNPCChannel.OnEventRaised += OnTalkedToNPC;

        _cameraService = IServiceLocator.Instance.GetService<CameraService>();
        if (_mainCamera == null)
            _mainCamera = _cameraService.Transform.gameObject;

        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;

        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        var locomotionState = new LocomotionState(this, _animator);
        var jumpState       = new JumpState(this, _animator, _stamina, _jumpStaminaCost);
        var attackState     = new PlayerAttackState(this, _animator);
        var hurtState       = new PlayerHurtState(this, _animator, _hurtDuration);
        var dieState        = new PlayerDieState(this, _animator, _deathLingerDuration);
        var danceState      = new PlayerDanceState(this, _animator, _danceDuration);

        At(locomotionState, jumpState, new FunctionPredicate(() => _grounded && _input.jump && _jumpTimeoutDelta <= 0f && (_stamina != null && _stamina.HasEnoughStamina(_jumpStaminaCost))));
        At(jumpState, locomotionState, new FunctionPredicate(() => _grounded && _verticalVelocity <= 0f));
        At(locomotionState, attackState, new FunctionPredicate(() => !_attackCooldownTimer.IsRunning && _input.attack && (_stamina != null && _stamina.HasEnoughStamina(_attackStaminaCost))));
        At(attackState, locomotionState, new FunctionPredicate(() => !_attackCooldownTimer.IsRunning));
        At(hurtState, locomotionState, new FunctionPredicate(() => !IsHurt));
        At(danceState, locomotionState, new FunctionPredicate(() => !IsDancing));
        Any(hurtState,  new FunctionPredicate(() => IsHurt && !IsDead));
        Any(dieState,   new FunctionPredicate(() => IsDead));
        Any(danceState, new FunctionPredicate(() => IsDancing && !IsDead && !IsHurt));
        Any(locomotionState, new FunctionPredicate(ReturnToLocomotionState));

        _stateMachine.SetState(locomotionState);
    }

    private void OnTalkedToNPC(Empty _)
    {
        if (GameManager.Instance.IsFinalBossKilled)
            IsDancing = true;
    }

    private bool ReturnToLocomotionState()
    {
        return _grounded
            && !_attackCooldownTimer.IsRunning
            && !_input.jump
            && !_input.attack
            && _verticalVelocity <= 0f
            && !IsHurt
            && !IsDead;
    }

    public void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        if (SceneController.IsLoading) return;

        if (_input.inventory)
        {
            _inventoryUIService ??= IServiceLocator.Instance.GetService<IInventoryUIService>() as InventoryUIService;
            _inventoryUIService?.Open();
        }
        GroundedCheck();
        if (_inputBlocked)
        {
            ApplyGravityOnly();
            return;
        } 
            
        _stateMachine.Update();
        _attackCooldownTimer.Tick(Time.deltaTime);
    }

    internal void HandleMovement()
    {
        float targetSpeed = _moveSpeed;
        if (_input.sprint)
        {
            if (_stamina != null && _stamina.HasEnoughStamina(_sprintStaminaCost * Time.deltaTime))
            {
                _stamina?.UseStamina(_sprintStaminaCost * Time.deltaTime);
                targetSpeed = _sprintSpeed;
            }
        }

        if (_input.move == Vector2.zero) targetSpeed = 0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _speedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(_input.move.x, 0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);

        if (_hasAnimator)
        {
            _animator.SetFloat(PlayerAnimIDs.Speed, _animationBlend);
            _animator.SetFloat(PlayerAnimIDs.MotionSpeed, inputMagnitude);
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
            _animator.SetBool(PlayerAnimIDs.Grounded, _grounded);
    }

    public void Attack()
    {
        if (!_attackCooldownTimer.IsRunning)
        {
            if(_stamina != null && !_stamina.HasEnoughStamina(_attackStaminaCost))
            {
                return;
            }
            _stamina?.UseStamina(_attackStaminaCost);
            _attackCooldownTimer.Start();
            if (_swordAttackAudioClips.Length > 0)
            {
                var index = Random.Range(0, _swordAttackAudioClips.Length);
                IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_swordAttackAudioClips[index], transform.TransformPoint(_controller.center), _swordAttackAudioVolume);
            }
            Vector3 attackPosition = transform.position;
            Collider[] hitColliders = Physics.OverlapSphere(attackPosition, _attackRange);
            var hitHealthComponents = new System.Collections.Generic.HashSet<Health>();
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    var health = hitCollider.GetComponent<Health>();
                    if (health != null && hitHealthComponents.Add(health))
                        health.TakeDamage(_attackDamage);
                }
            }

        }
    }

    private void ApplyGravityOnly()
    {
        if (_grounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
            if (_verticalVelocity > -TerminalVelocity)
                _verticalVelocity = _gravity * Time.deltaTime;
        }
        _controller.Move(new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = _grounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
        Vector3 attackPosition = transform.position + transform.forward;
        Gizmos.DrawSphere(attackPosition, _attackRange);
    }

    private void HandleHeartbeat(float percentage)
    {
        if (percentage > 0f && percentage < _lowHealthThreshold)
        {
            if (!_heartbeatSource.isPlaying)
                _heartbeatSource.Play();
        }
        else
        {
            StopHeartbeat();
        }
    }

    private void StopHeartbeat()
    {
        if (_heartbeatSource != null && _heartbeatSource.isPlaying)
            _heartbeatSource.Stop();
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (_footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, _footstepAudioClips.Length);
                IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_footstepAudioClips[index], transform.TransformPoint(_controller.center), _footstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.1f)
            IServiceLocator.Instance.GetService<ISoundService>()?.PlayOneShot(_landingAudioClip, transform.TransformPoint(_controller.center), _footstepAudioVolume);
    }

    private IEnumerator UnblockNextFrame()
    {
        _input.attack = false;
        yield return null;
        _inputBlocked = false;
        _input.enabled = true;
    }
}
