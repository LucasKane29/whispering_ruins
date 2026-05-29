using UnityEngine;

public static class PlayerAnimIDs
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Grounded = Animator.StringToHash("Grounded");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int FreeFall = Animator.StringToHash("FreeFall");
    public static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Hurt   = Animator.StringToHash("GetHit01_THS");
    public static readonly int Die    = Animator.StringToHash("Die01_THS");
}
