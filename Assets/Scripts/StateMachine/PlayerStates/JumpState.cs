using UnityEngine;

public class JumpState : BaseState<PlayerController>
{
    private Stamina _stamina;
    private float _staminaCost;
    public JumpState(PlayerController controller, Animator animator, Stamina stamina, float staminaCost) : base(controller, animator)
    {
        _stamina = stamina;
        _staminaCost = staminaCost;
    }

    public override void OnEnter()
    {
        _stamina?.UseStamina(_staminaCost);

        agent.VerticalVelocity = Mathf.Sqrt(agent.JumpHeight * -2f * agent.Gravity);

        if (agent.HasAnimator)
            animator.SetBool(PlayerAnimIDs.Jump, true);
    }

    public override void OnExit()
    {
        agent.Input.jump = false;
    }

    public override void Update()
    {
        agent.JumpTimeoutDelta = agent.JumpTimeout;

        if (agent.VerticalVelocity <= 0f)
        {
            if (agent.FallTimeoutDelta >= 0f)
                agent.FallTimeoutDelta -= Time.deltaTime;
            else if (agent.HasAnimator)
                animator.SetBool(PlayerAnimIDs.FreeFall, true);
        }

        agent.Input.jump = false;

        if (agent.VerticalVelocity < PlayerController.TerminalVelocity)
            agent.VerticalVelocity += agent.Gravity * Time.deltaTime;

        agent.HandleMovement();
    }
}
