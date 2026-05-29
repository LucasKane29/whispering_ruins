using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private string _spawnId;
    public string SpawnId => _spawnId;  

    public Vector3 Position => transform.position;
}
