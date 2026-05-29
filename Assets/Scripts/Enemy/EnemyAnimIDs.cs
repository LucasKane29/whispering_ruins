using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyAnimIDs
{
    public static readonly int Idle = Animator.StringToHash("IdleNormal");
    public static readonly int Run = Animator.StringToHash("RunFWD");
    public static readonly int Walk = Animator.StringToHash("WalkFWD");
    public static readonly int Attack = Animator.StringToHash("Attack01");
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int Hurt = Animator.StringToHash("GetHit");

}
