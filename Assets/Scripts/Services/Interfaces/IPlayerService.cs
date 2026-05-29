using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerService : IService
{
    Transform Transform { get; }
    Health Health { get; }
    Stamina Stamina { get; }
}
