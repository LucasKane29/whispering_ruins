using UnityEngine;

public class PlayerDieState : BaseState<PlayerController>
{
    private bool _gameOverTriggered;

    public PlayerDieState(PlayerController controller, Animator animator) : base(controller, animator) { }

    public override void OnEnter()
    {
        _gameOverTriggered = false;
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

        if (_gameOverTriggered || !agent.HasAnimator) return;

        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.shortNameHash == PlayerAnimIDs.Die && info.normalizedTime >= 1f && !animator.IsInTransition(0))
        {
            _gameOverTriggered = true;
            GameManager.Instance.TriggerGameOver();
        }
    }
}
