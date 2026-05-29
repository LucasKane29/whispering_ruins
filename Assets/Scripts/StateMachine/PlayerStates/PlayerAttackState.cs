using UnityEngine;

public class PlayerAttackState : BaseState<PlayerController>
{
    public PlayerAttackState(PlayerController controller, Animator animator) : base(controller, animator) { }

    public override void OnEnter()
    {
        if (agent.HasAnimator)
            animator.SetTrigger(PlayerAnimIDs.Attack);
        agent.Attack();
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
