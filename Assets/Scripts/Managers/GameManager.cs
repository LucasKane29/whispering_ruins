using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct PlayerData
{
    public float health;
    public float stamina;
    public int souls;
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntEventChannel _soulCountChannel;
    [SerializeField] private IntEventChannel _onSoulCountChangedChannel;

    public PlayerData SavedPlayerData;
    public static GameManager Instance { get; private set; }
    public int Souls { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (_soulCountChannel != null)
            _soulCountChannel.OnEventRaised += AddSouls;
    }
    private void OnDestroy()
    {
        if (_soulCountChannel != null)
            _soulCountChannel.OnEventRaised -= AddSouls;
    }

    public void AddSouls(int amount)
    {
        Souls += amount;
        _onSoulCountChangedChannel?.Invoke(amount);
    }

    public void SetSouls(int amount)
    {
        int delta = amount - Souls;
        Souls = amount;
        if (delta != 0)
            _onSoulCountChangedChannel?.Invoke(delta);
    }

    public void SavePlayer(float health, float stamina)
    {
        SavedPlayerData = new PlayerData();
        SavedPlayerData.health = health;
        SavedPlayerData.stamina = stamina;
        SavedPlayerData.souls = Souls;
    }

    public void TriggerGameOver()
    {
        string currentSlot = SceneController.Instance.GetSlotForActiveScene();

        var plan = SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.GameOver, SceneDatabase.Scenes.GameOver, setActive: true)
            .WithOverlay();

        foreach (var slot in SceneController.Instance.GetLoadedSlots())
        {
            if (slot == SceneDatabase.Slots.Core) continue;
            plan.Unload(slot);
        }

        plan.Perform();
    }
}
