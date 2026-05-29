using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private GameObject _activePlayer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SpawnOrMovePlayer(PlayerSpawnPoint target, Scene scene)
    {
        if (playerPrefab == null || target == null) return;

        _activePlayer = Instantiate(playerPrefab, target.Position, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(_activePlayer, scene);

        var saveService = IServiceLocator.Instance.GetService<ISaveService>() as SaveService;
        var pending = saveService?.GetPendingLoad();
        if (pending == null) return;

        GameManager.Instance.SetSouls(pending.souls);

        var inventory = IServiceLocator.Instance.GetService<IInventoryService>();
        inventory?.Clear();
        if (saveService.ItemDatabase != null)
        {
            foreach (var s in pending.inventory)
            {
                var item = saveService.ItemDatabase.GetById(s.itemId);
                if (item != null)
                    inventory?.Add(item, s.count);
            }
        }

        StartCoroutine(RestoreParametersNextFrame(pending.health, pending.stamina));

        saveService.ClearPendingLoad();
    }

    private IEnumerator RestoreParametersNextFrame(float health, float stamina)
    {
        var healthComp  = _activePlayer != null ? _activePlayer.GetComponentInChildren<Health>()  : null;
        var staminaComp = _activePlayer != null ? _activePlayer.GetComponentInChildren<Stamina>() : null;

        yield return null;

        healthComp?.Initialize(health);
        staminaComp?.Initialize(stamina);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] spawnPoints = scene.GetRootGameObjects();
        if (spawnPoints.Length == 0) return;
        foreach (var spawnPoint in spawnPoints)
        {
            var spawnPointComponent = spawnPoint.GetComponentInChildren<PlayerSpawnPoint>();
            if (spawnPointComponent != null)
            {
                SpawnOrMovePlayer(spawnPointComponent, scene);
                return;
            }
        }
    }
}
