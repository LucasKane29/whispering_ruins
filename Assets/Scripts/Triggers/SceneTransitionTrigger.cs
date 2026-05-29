using UnityEngine;

public class SceneTransitionTrigger : Interactable
{
    [SceneConstant(typeof(SceneDatabase.Slots))]
    [SerializeField] private string _targetSlot;
    [SceneConstant(typeof(SceneDatabase.Scenes))]
    [SerializeField] private string _targetScene;
    [SceneConstant(typeof(SceneDatabase.Slots))]
    [SerializeField] private string _previousSlot;

    public void OnInteract()
    {
        SceneController.Instance
            .NewTransitions()
            .Load(_targetSlot, _targetScene, setActive: true)
            .Unload(_previousSlot)
            .WithOverlay()
            .WithClearUnusedAssets()
            .Perform();
    }
}
