using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPauseService : IService
{
    event Action OnPaused;
    event Action OnResumed;

    bool IsPaused { get;}
    void Pause();
    void Resume();
    void TogglePause();
    void GoToMainMenu();
}
