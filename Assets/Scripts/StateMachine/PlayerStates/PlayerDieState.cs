using UnityEngine;

public class PlayerDieState : BaseState<PlayerController>
{
    private readonly CountdownTimer _lingerTimer;
    private bool _animationFinished;

    public PlayerDieState(PlayerController controller, Animator animator, float lingerDuration) : base(controller, animator)
    {
        _lingerTimer = new CountdownTimer(lingerDuration);
        _lingerTimer.OnTimerStop += () => GameManager.Instance.TriggerGameOver();
    }

    public override void OnEnter()
    {
        _animationFinished = false;
        if (agent.HasAnimator)
            animator.CrossFade(PlayerAnimIDs.Die, crossFadeDuration);
    }

    public override void Update()
    {
        if (agent.Grounded)
            agent.VerticalVelocity = -2f;
        else if (agent.VerticalVelocity < PlayerController.TerminalVelocity)
            agent.VerticalVelocity += agent.Gravity * Time.deltaTime;

        agent.Controller.Move(new Vector3(0f, agent.VerticalVelocity, 0f) * Time.deltaTime);

        _lingerTimer.Tick(Time.deltaTime);

        if (_animationFinished || !agent.HasAnimator) return;

        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.shortNameHash == PlayerAnimIDs.Die && info.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            _animationFinished = true;
            _lingerTimer.Start();
        }
    }
}
