using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
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


    public void OnContinueClick()
    {
        IServiceLocator.Instance.GetService<ISaveService>()?.Load();
    }

    public void OnRestartClick()
    {
        SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.Hub, SceneDatabase.Scenes.Hub, setActive: true)
            .Unload(SceneDatabase.Slots.GameOver)
            .WithOverlay()
            .WithoutSave()
            .Perform();
    }

    public void OnMainMenuClick()
    {
        SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu, setActive: true)
            .Unload(SceneDatabase.Slots.GameOver)
            .WithOverlay()
            .WithoutSave()
            .Perform();
    }
}