using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController Instance;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion
    [SerializeField] private LoadingOverlay _loadingOverlay;
    private Dictionary<string, string> _loadedScene = new();
    private string _activeSlot;
    private bool _isBusy = false;

    public IEnumerable<string> GetLoadedSlots() => _loadedScene.Keys.ToList();

    public SceneTransitionPlan NewTransitions()
    {
        return new SceneTransitionPlan();
    }

    public Coroutine ExecutePlane(SceneTransitionPlan plan)
    {
        if (_isBusy) return null;
        _isBusy = true;
        return StartCoroutine(ChangeSceneRoutine(plan));
    }

    private IEnumerator ChangeSceneRoutine(SceneTransitionPlan plan)
    {
        IServiceLocator.Instance.GetService<ISaveService>()?.PrepareTransit();

        if(plan.Overlay)
        {
            yield return _loadingOverlay.FadeInBlack();
            yield return new WaitForSeconds(0.5f);
        }
        foreach (var scene in plan.ScenesToUnload)
        {
            yield return UnloadSceneRoutine(scene);
        }
        if (plan.ClearUnusedAssets)
            yield return CleanupUnusedAssetsRoutine();
        foreach (var kvp in plan.ScenesToLoad)
        {
            if (_loadedScene.ContainsKey(kvp.Key))
                yield return UnloadSceneRoutine(kvp.Key);
            yield return LoadAdditiveRoutine(kvp.Key, kvp.Value, plan.ActiveSceneName == kvp.Value);
        }

        if(plan.Overlay)
        {
            yield return _loadingOverlay.FadeOutBlack();
        }

        IServiceLocator.Instance.GetService<ISaveService>()?.Save();

        _isBusy = false;
    }

    private IEnumerator LoadAdditiveRoutine(string slotKey, string sceneName, bool setActive)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        if (loadOperation == null) yield break;
        loadOperation.allowSceneActivation = false;
        while (loadOperation.progress < 0.9f)
        {
            yield return null;
        }
        loadOperation.allowSceneActivation = true;
        if (setActive)
        {
            Scene newScene = SceneManager.GetSceneByName(sceneName);
            if(newScene.IsValid() && newScene.isLoaded)
                SceneManager.SetActiveScene(newScene);
            _activeSlot = slotKey;
        }
        _loadedScene.Add(slotKey, sceneName);
    }

    private IEnumerator UnloadSceneRoutine(string slotKey)
    {
        if(!_loadedScene.TryGetValue(slotKey, out string sceneName)) yield break;
        if(string.IsNullOrEmpty(sceneName)) yield break;

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);
        if (unloadOperation != null)
        {
            while (!unloadOperation.isDone)
            {
                yield return null;
            }
        }

        _loadedScene.Remove(slotKey);
    }

    private IEnumerator CleanupUnusedAssetsRoutine()
    {
        AsyncOperation cleanOperation = Resources.UnloadUnusedAssets();
        if (cleanOperation != null)
        {
            while (!cleanOperation.isDone)
            {
                yield return null;
            }
        }
    }

    public class SceneTransitionPlan
    {
        public Dictionary<string, string> ScenesToLoad { get; } = new();
        public List<string> ScenesToUnload { get; } = new();
        public string ActiveSceneName { get; private set; } = string.Empty;
        public bool ClearUnusedAssets { get; private set; } = false;
        public bool Overlay { get; private set; } = false;
        public SceneTransitionPlan Load(string slotKey, string sceneName, bool setActive = false)
        {
            ScenesToLoad.Add(slotKey, sceneName);
            if (setActive)
            {
                ActiveSceneName = sceneName;
            }
            return this;
        }

        public SceneTransitionPlan Unload(string sceneName)
        {
            ScenesToUnload.Add(sceneName);
            return this;
        }

        public SceneTransitionPlan WithOverlay()
        {
            Overlay = true;
            return this;
        }

        public SceneTransitionPlan WithClearUnusedAssets()
        {
            ClearUnusedAssets = true;
            return this;
        }

        public Coroutine Perform()
        {
            return SceneController.Instance.ExecutePlane(this);
        }

    }

    public string GetSlotForActiveScene() => _activeSlot;

    public string GetActiveSceneName()
    {
        if (_activeSlot == null) return null;
        return _loadedScene.TryGetValue(_activeSlot, out var name) ? name : null;
    }

}
