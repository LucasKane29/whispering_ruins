using UnityEngine;

public static class BossAnimIDs
{
    public static readonly int Idle = Animator.StringToHash("IdleNormal");
    public static readonly int Run = Animator.StringToHash("Run");
    public static readonly int MeleeAttack = Animator.StringToHash("Attack01");
    public static readonly int MediumRangeAttack = Animator.StringToHash("Attack02");
    public static readonly int RangedAttack = Animator.StringToHash("Attack03");
    public static readonly int AOEAttack = Animator.StringToHash("Attack05RPT");
    public static readonly int PhaseTransition = Animator.StringToHash("Taunting");
    public static readonly int Hurt = Animator.StringToHash("GetHit");
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int Burrow = Animator.StringToHash("GroundDiveIn");
    public static readonly int Emerge = Animator.StringToHash("GroundBreakThrough");
}
