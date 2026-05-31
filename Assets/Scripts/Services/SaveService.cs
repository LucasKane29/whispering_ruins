using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveService : MonoBehaviour, ISaveService
{
    [SerializeField] public ItemDatabase ItemDatabase;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private SaveData _pendingLoad;
    private readonly HashSet<string> _clearedSpawners = new();

    public bool HasSave { get; private set; }

    private void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<ISaveService, SaveService>(this);
        HasSave = File.Exists(SavePath);
    }

    private void OnDestroy()
    {
        if (IServiceLocator.Instance != null)
            IServiceLocator.Instance.TryUnregisterService<ISaveService, SaveService>(this);
    }

    public void Save()
    {
        var inventory = IServiceLocator.Instance.GetService<IInventoryService>();
        var player    = IServiceLocator.Instance.GetService<IPlayerService>();

        if (player == null) return;

        var sceneSlot = SceneController.Instance.GetSlotForActiveScene();
        var sceneName = SceneController.Instance.GetActiveSceneName();

        if (string.IsNullOrEmpty(sceneSlot) || string.IsNullOrEmpty(sceneName)) return;

        var data = new SaveData
        {
            sceneSlot = sceneSlot,
            sceneName = sceneName,
            health    = player.Health.CurrentHealth,
            stamina   = player.Stamina.CurrentStamina,
            souls     = GameManager.Instance.Souls,
        };

        if (inventory != null)
        {
            foreach (var slot in inventory.Slots)
            {
                if (string.IsNullOrEmpty(slot.Item.id)) continue;
                data.inventory.Add(new SavedInventorySlot { itemId = slot.Item.id, count = slot.count });
            }
        }

        data.clearedSpawners = new List<string>(_clearedSpawners);

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
        HasSave = true;
    }

    public void Load()
    {
        if (!HasSave) return;

        var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));

        if (string.IsNullOrEmpty(data.sceneSlot) || string.IsNullOrEmpty(data.sceneName)) return;

        _pendingLoad = data;
        _clearedSpawners.Clear();
        foreach (var id in data.clearedSpawners)
            _clearedSpawners.Add(id);

        var plan = SceneController.Instance
            .NewTransitions()
            .Load(data.sceneSlot, data.sceneName, setActive: true)
            .WithOverlay();

        foreach (var slot in SceneController.Instance.GetLoadedSlots())
        {
            if (slot == SceneDatabase.Slots.Core) continue;
            plan.Unload(slot);
        }

        plan.Perform();
    }

    public void PrepareTransit()
    {
        var player = IServiceLocator.Instance.GetService<IPlayerService>();
        if (player == null || player.Health == null || player.Health.IsDead) return;

        var inventory = IServiceLocator.Instance.GetService<IInventoryService>();

        var data = new SaveData
        {
            health  = player.Health.CurrentHealth,
            stamina = player.Stamina?.CurrentStamina ?? 0f,
            souls   = GameManager.Instance.Souls,
            clearedSpawners = new List<string>(_clearedSpawners),
        };

        if (inventory != null)
        {
            foreach (var slot in inventory.Slots)
            {
                if (string.IsNullOrEmpty(slot.Item.id)) continue;
                data.inventory.Add(new SavedInventorySlot { itemId = slot.Item.id, count = slot.count });
            }
        }

        _pendingLoad = data;
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
            File.Delete(SavePath);
        HasSave = false;
        _pendingLoad = null;
        _clearedSpawners.Clear();
        GameManager.Instance.SetSouls(0);
    }

    public bool IsSpawnerCleared(string id) => _clearedSpawners.Contains(id);

    public void MarkSpawnerCleared(string id) => _clearedSpawners.Add(id);

    public SaveData GetPendingLoad() => _pendingLoad;

    public void ClearPendingLoad() => _pendingLoad = null;
}
