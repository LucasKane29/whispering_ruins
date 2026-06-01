using System.Collections;
using UnityEngine;

public class HubEndingController : MonoBehaviour
{
    [SerializeField] private EventChannel _onNpcTalked;
    [SerializeField] private Material _postBossSkybox;
    [SerializeField] private DialogueSO _postBossDialogue;
    [SerializeField] private NPC _npc;
    [SerializeField] private Animator _npcAnimator;
    [SerializeField] private AudioClip _epicMusic;
    [SerializeField] private string _npcEndingTrigger = "Ending";
    [SerializeField] private float _animationDuration = 3f;
    [SerializeField] private float _musicFadeDuration = 1.5f;
    [SerializeField] private Light[] _lightsToEnable;
    [SerializeField] private GameObject _birdsAmbient;

    private IEnumerator Start()
    {
        if (!GameManager.Instance.IsFinalBossKilled) yield break;

        if (_birdsAmbient != null)
        {
            _birdsAmbient.SetActive(GameManager.Instance != null && GameManager.Instance.IsFinalBossKilled);
        }

        _npc.SetDialogue(_postBossDialogue);
        _onNpcTalked.OnEventRaised += OnNpcTalked;

        foreach (var light in _lightsToEnable)
        {
            if (light == null) continue;
            light.gameObject.SetActive(true);
            light.enabled = true;
        }

        yield return null; // чекаємо SetActiveScene щоб RenderSettings не перезаписались

        if (_postBossSkybox != null)
        {
            RenderSettings.skybox = _postBossSkybox;
            DynamicGI.UpdateEnvironment();
        }
    }

    private void OnDestroy()
    {
        _onNpcTalked.OnEventRaised -= OnNpcTalked;
    }

    private void OnNpcTalked(Empty _)
    {
        _onNpcTalked.OnEventRaised -= OnNpcTalked;
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
        _npcAnimator.SetTrigger(_npcEndingTrigger);

        IServiceLocator.Instance.GetService<ISoundService>()
            .PlayMusic(_epicMusic, _musicFadeDuration);

        yield return new WaitForSeconds(_animationDuration);

        yield return SceneController.Instance.FadeToBlack();

        SceneController.Instance.NewTransitions()
            .Load(SceneDatabase.Slots.Credits, SceneDatabase.Scenes.Credits, setActive: true)
            .WithOverlay()
            .WithoutMinimumDisplay()
            .WithoutSave()
            .Perform();
    }
}
