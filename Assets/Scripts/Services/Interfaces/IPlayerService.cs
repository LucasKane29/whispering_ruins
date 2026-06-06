using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerService : IService
{
    Transform Transform { get; }
    Health Health { get; }
    Stamina Stamina { get; }
    void ApplyKnockback(Vector3 force);
}
