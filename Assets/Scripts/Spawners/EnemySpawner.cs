using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public GameObject dropPrefab;
}

public class EnemySpawner : BaseSpawner
{
    [SerializeField] private string _spawnerId;
    [SerializeField] private List<EnemySpawnData> _enemySpawnDataList;

    public event Action OnAllEnemiesDead;

    private int _aliveCount;
    private bool _hasSpawned;

    private string UniqueId => $"{gameObject.scene.name}/{_spawnerId}";

    public override void Spawn()
    {
        if (_hasSpawned || _enemySpawnDataList.Count == 0) return;

        if (!string.IsNullOrEmpty(_spawnerId))
        {
            var saveService = IServiceLocator.Instance.GetService<ISaveService>();
            if (saveService != null && saveService.IsSpawnerCleared(UniqueId))
            {
                _hasSpawned = true;
                OnAllEnemiesDead?.Invoke();
                return;
            }
        }

        _hasSpawned = true;

        foreach (var spawnData in _enemySpawnDataList)
        {
            var enemy = Instantiate(spawnData.enemyPrefab, spawnData.spawnPoint.position, spawnData.spawnPoint.rotation);
            SceneManager.MoveGameObjectToScene(enemy, gameObject.scene);

            var capturedEnemy = enemy;
            var capturedDrop = spawnData.dropPrefab;
            var health = enemy.GetComponent<Health>();
            if (health != null)
            {
                _aliveCount++;
                var controller = enemy.GetComponent<EnemyController>();
                if (controller != null)
                    controller.OnDeathAnimationComplete += () => HandleEnemyDeath(capturedEnemy.transform.position, capturedDrop);
            }
        }
    }

    private void HandleEnemyDeath(Vector3 position, GameObject dropPrefab)
    {
        _aliveCount--;
        if (dropPrefab != null)
            Instantiate(dropPrefab, position, Quaternion.identity);

        if (_aliveCount <= 0)
        {
            if (!string.IsNullOrEmpty(_spawnerId))
                IServiceLocator.Instance.GetService<ISaveService>()?.MarkSpawnerCleared(UniqueId);

            OnAllEnemiesDead?.Invoke();
        }
    }
}
