using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneManager : MonoBehaviour
{
    void Start()
    {
        SceneController.Instance
            .NewTransitions()
            .Load(SceneDatabase.Slots.MainMenu, SceneDatabase.Scenes.MainMenu)
            .Perform();
    }
}
