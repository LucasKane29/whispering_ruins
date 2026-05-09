using UnityEngine;

public abstract partial class BaseState<TAgent> : IState
{
    protected readonly TAgent agent;
    protected readonly Animator animator;

    protected float crossFadeDuration = 0.1f;

    public BaseState(TAgent agent, Animator animator)
    {
        this.agent = agent;
        this.animator = animator;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnExit() { }
}
