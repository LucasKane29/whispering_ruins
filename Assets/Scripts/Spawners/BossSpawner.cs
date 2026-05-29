using UnityEngine;
using UnityEngine.SceneManagement;

public class BossSpawner : BaseSpawner
{
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private BossTrigger _bossTrigger;
    [SerializeField] private BossHealthBar _bossHealthBar;

    private bool _hasSpawned;

    public override void Spawn()
    {
        if (_hasSpawned) return;
        _hasSpawned = true;

        var bossObject = Instantiate(_bossPrefab, _spawnPoint.position, _spawnPoint.rotation);
        SceneManager.MoveGameObjectToScene(bossObject, gameObject.scene);
        bossObject.SetActive(true);
        var controller = bossObject.GetComponent<BossController>();
        _bossTrigger.Init(controller);
        _bossHealthBar.Init(controller);
    }
}
