using UnityEngine;

public class BossIdleState : BaseState<BossController>
{
    private readonly PlayerDetector _playerDetector;

    public BossIdleState(BossController agent, Animator animator, PlayerDetector playerDetector) : base(agent, animator)
    {
        _playerDetector = playerDetector;
    }

    public override void OnEnter()
    {
        animator.CrossFade(BossAnimIDs.Idle, crossFadeDuration);
    }

    public override void Update()
    {
        if (_playerDetector.Player == null) return;

        var direction = (_playerDetector.Player.position - agent.transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;

        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 5f);
    }
}
