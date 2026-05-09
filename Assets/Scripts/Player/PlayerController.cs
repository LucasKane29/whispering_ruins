using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IStatable
{
    [Header("Гравець")]
    [Tooltip("Швидкість ходи персонажу у м/с")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Швидкість бігу персонажу у м/с")]
    public float SprintSpeed = 5.335f;

    [Tooltip("Наскільки швидко персонаж змінює напрямок")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Прискорення та сповільнення")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("Висота стрибку")]
    public float JumpHeight = 1.2f;

    [Tooltip("Персонаж використовує власне значення гравітації. Значення за замовчуванням -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Час затримки перед стрибком")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Час, необхідний для переходу в стан падіння. Корисно для спуску по сходах")]
    public float FallTimeout = 0.15f;

    [Header("Приземлення персонажу")]
    [Tooltip("Флаг показує чи на землі персонаж")]
    public bool Grounded = true;

    [Tooltip("Корисно для грубих поверхонь")]
    public float GroundedOffset = -0.14f;

    [Tooltip("Радіус перевірки на дотик до землі. Повинен відповідати радіусу CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("Які шари персонаж використовує як землю")]
    public LayerMask GroundLayers;

    [Header("Cinemachine")]
    [Tooltip("Ціль для слідування, встановлена в Cinemachine Virtual Camera, за якою камера буде слідувати")]
    public GameObject CinemachineCameraTarget;

    internal const float TerminalVelocity = 53.0f;
    internal float VerticalVelocity;
    internal float Speed;
    internal float AnimationBlend;
    internal float TargetRotation;
    internal float RotationVelocity;
    internal float JumpTimeoutDelta;
    internal float FallTimeoutDelta;

    internal Animator Animator;
    internal CharacterController Controller;
    internal PlayerInputs Input;
    internal GameObject MainCamera;
    internal bool HasAnimator;

    private StateMachine _stateMachine;

    private void Awake()
    {
        if (MainCamera == null)
            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {

        HasAnimator = TryGetComponent(out Animator);
        Controller = GetComponent<CharacterController>();
        Input = GetComponent<PlayerInputs>();
        JumpTimeoutDelta = JumpTimeout;
        FallTimeoutDelta = FallTimeout;

        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        _stateMachine = new StateMachine();
        var locomotionState = new LocomotionState(this, Animator);
        var jumpState = new JumpState(this, Animator);

        At(locomotionState, jumpState, new FunctionPredicate(() => Grounded && Input.jump && JumpTimeoutDelta <= 0f));
        At(jumpState, locomotionState, new FunctionPredicate(() => Grounded));

        _stateMachine.SetState(locomotionState);
    }

    public void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
    public void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

    private void Update()
    {
        HasAnimator = TryGetComponent(out Animator);
        GroundedCheck();
        _stateMachine.Update();
    }

    internal void HandleMovement()
    {
        float targetSpeed = Input.sprint ? SprintSpeed : MoveSpeed;
        if (Input.move == Vector2.zero) targetSpeed = 0f;

        float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0f, Controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = Input.analogMovement ? Input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            Speed = Mathf.Round(Speed * 1000f) / 1000f;
        }
        else
        {
            Speed = targetSpeed;
        }

        AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (AnimationBlend < 0.01f) AnimationBlend = 0f;

        Vector3 inputDirection = new Vector3(Input.move.x, 0f, Input.move.y).normalized;

        if (Input.move != Vector2.zero)
        {
            TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f, TargetRotation, 0f) * Vector3.forward;
        Controller.Move(targetDirection.normalized * (Speed * Time.deltaTime) + new Vector3(0f, VerticalVelocity, 0f) * Time.deltaTime);

        if (HasAnimator)
        {
            Animator.SetFloat(PlayerAnimIDs.Speed, AnimationBlend);
            Animator.SetFloat(PlayerAnimIDs.MotionSpeed, inputMagnitude);
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        if (HasAnimator)
            Animator.SetBool(PlayerAnimIDs.Grounded, Grounded);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Gizmos.color = Grounded ? transparentGreen : transparentRed;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        Debug.Log(animationEvent.animatorClipInfo.weight);
        if (animationEvent.animatorClipInfo.weight > 0.1f)
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(Controller.center), FootstepAudioVolume);
    }
}
