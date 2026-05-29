using UnityEngine;

public class LocomotionState : BaseState<PlayerController>
{
    public LocomotionState(PlayerController controller, Animator animator) : base(controller, animator) { }

    public override void OnEnter() { }

    public override void Update()
    {
        agent.FallTimeoutDelta = agent.FallTimeout;

        if (agent.HasAnimator)
        {
            animator.SetBool(PlayerAnimIDs.Jump, false);
            animator.SetBool(PlayerAnimIDs.FreeFall, false);
        }

        if (agent.VerticalVelocity < 0f)
            agent.VerticalVelocity = -2f;

        if (agent.JumpTimeoutDelta >= 0f)
            agent.JumpTimeoutDelta -= Time.deltaTime;

        if (agent.VerticalVelocity < PlayerController.TerminalVelocity)
            agent.VerticalVelocity += agent.Gravity * Time.deltaTime;

        agent.HandleMovement();
    }
}
