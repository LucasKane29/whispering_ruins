using UnityEngine;

public abstract class BossBaseState : BaseState<BossController>
{
    protected readonly PlayerDetector playerDetector;
    protected BossBaseState(BossController agent, Animator animator, PlayerDetector playerDetector): base(agent, animator)
    {
        this.playerDetector = playerDetector;
    }

    protected void FacePlayer()
    {
        var direction = (playerDetector.Player.position - agent.transform.position).normalized;
        direction.y = 0f;
        if (direction == Vector3.zero) return;
        agent.transform.rotation = Quaternion.Slerp(
            agent.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * 10f);
    }
}
