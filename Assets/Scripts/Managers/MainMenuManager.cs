using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button _continueButton;

    private void Start()
    {
        if (_continueButton != null)
        {
            var hasSave = IServiceLocator.Instance.GetService<ISaveService>()?.HasSave ?? false;
            _continueButton.interactable = hasSave;
        }
    }

    public void OnNewGameClick()
    {
        IServiceLocator.Instance.GetService<ISaveService>()?.DeleteSave();
        SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.Hub, SceneDatabase.Scenes.Hub, setActive: true)
            .Unload(SceneDatabase.Slots.MainMenu)
            .WithOverlay()
            .Perform();
    }

    public void OnContinueClick()
    {
        IServiceLocator.Instance.GetService<ISaveService>()?.Load();
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
