using UnityEngine;

public class JumpState : BaseState<PlayerController>
{
    public JumpState(PlayerController controller, Animator animator) : base(controller, animator) { }

    public override void OnEnter()
    {
        agent.VerticalVelocity = Mathf.Sqrt(agent.JumpHeight * -2f * agent.Gravity);

        if (agent.HasAnimator)
            animator.SetBool(PlayerAnimIDs.Jump, true);

        Debug.Log("Entered Jump State");
    }

    public override void Update()
    {
        agent.JumpTimeoutDelta = agent.JumpTimeout;

        if (agent.FallTimeoutDelta >= 0f)
            agent.FallTimeoutDelta -= Time.deltaTime;
        else if (agent.HasAnimator)
            animator.SetBool(PlayerAnimIDs.FreeFall, true);

        agent.Input.jump = false;

        if (agent.VerticalVelocity < PlayerController.TerminalVelocity)
            agent.VerticalVelocity += agent.Gravity * Time.deltaTime;

        agent.HandleMovement();
    }
}
