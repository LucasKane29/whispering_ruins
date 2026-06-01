using UnityEngine;

public class PlayerDanceState : BaseState<PlayerController>
{
    private readonly CountdownTimer _timer;

    public PlayerDanceState(PlayerController controller, Animator animator, float duration) : base(controller, animator)
    {
        _timer = new CountdownTimer(duration);
        _timer.OnTimerStop += () => agent.IsDancing = false;
    }

    public override void OnEnter()
    {
        if (agent.HasAnimator)
            animator.CrossFade(PlayerAnimIDs.Dance, crossFadeDuration);
        _timer.Start();
    }

    public override void Update()
    {
        _timer.Tick(Time.deltaTime);

        if (agent.Grounded)
            agent.VerticalVelocity = -2f;
        else if (agent.VerticalVelocity < PlayerController.TerminalVelocity)
            agent.VerticalVelocity += agent.Gravity * Time.deltaTime;

        agent.Controller.Move(new Vector3(0f, agent.VerticalVelocity, 0f) * Time.deltaTime);
    }

    public override void OnExit() => _timer.Stop();
}
