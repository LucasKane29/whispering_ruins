using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseService : MonoBehaviour, IPauseService
{
    public event Action OnPaused;
    public event Action OnResumed;

    [SerializeField] private GameObject _pauseMenuUI;
    [SerializeField] private string[] _excludedScenes;

    private HashSet<string> _excludedScenesSet;
    public bool IsPaused { get; private set; }

    private static readonly HashSet<string> _nonGameplayScenes = new()
    {
        SceneDatabase.Scenes.MainMenu,
        SceneDatabase.Scenes.GameOver,
        SceneDatabase.Scenes.Credits,
    };

    void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<IPauseService, PauseService>(this);
        _excludedScenesSet = new HashSet<string>(_excludedScenes ?? System.Array.Empty<string>());
    }

    void OnDestroy()
    {
        if(IServiceLocator.Instance != null)
            IServiceLocator.Instance.TryUnregisterService<IPauseService, PauseService>(this);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            var inventoryUI = IServiceLocator.Instance.GetService<IInventoryUIService>();
            if (inventoryUI == null || !inventoryUI.IsOpen)
                TogglePause();
        }
    }

    public void Pause()
    {
        if (SceneController.IsLoading) return;
        var currentScene = SceneController.Instance.GetActiveSceneName() ?? string.Empty;
        if (_nonGameplayScenes.Contains(currentScene) || _excludedScenesSet.Contains(currentScene))
            return;
        Time.timeScale = 0f;
        _pauseMenuUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
        OnPaused?.Invoke();
       
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        _pauseMenuUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsPaused = false;
        OnResumed?.Invoke();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else Pause();
    }

    public void GoToMainMenu()
    {
        IServiceLocator.Instance.GetService<ISaveService>()?.Save();

        Time.timeScale = 1f;

        var plan = SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu, setActive: true)
            .WithOverlay()
            .WithoutSave();

        foreach (var slot in SceneController.Instance.GetLoadedSlots())
        {
            if (slot == SceneDatabase.Slots.Core) continue;
            plan.Unload(slot);
        }

        plan.Perform();
        _pauseMenuUI.SetActive(false);
    }
}
