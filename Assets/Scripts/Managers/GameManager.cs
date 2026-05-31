using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private IntEventChannel _soulCountChannel;
    [SerializeField] private IntEventChannel _onSoulCountChangedChannel;
    [SerializeField] private EventChannel _onFinalBossKilled;

    private bool _isFinalBossKilled = false;

    public bool IsFinalBossKilled => _isFinalBossKilled;

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
        if (_onFinalBossKilled != null)
            _onFinalBossKilled.OnEventRaised += SetFinalBossKilled;
    }
    private void OnDestroy()
    {
        if (_soulCountChannel != null)
            _soulCountChannel.OnEventRaised -= AddSouls;
        if (_onFinalBossKilled != null)
            _onFinalBossKilled.OnEventRaised -= SetFinalBossKilled;
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

    public void SetFinalBossKilled(Empty empty)
    {
        _isFinalBossKilled = true;
    }
}
