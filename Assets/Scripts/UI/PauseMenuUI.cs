using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    private PauseService _pauseService;

    private void Start()
    {
        _pauseService = IServiceLocator.Instance.GetService<IPauseService>() as PauseService;
    }

    public void OnResumeClicked() => _pauseService?.Resume();
    public void OnMainMenuClicked() => _pauseService?.GoToMainMenu();
}
