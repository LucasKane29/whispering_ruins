using UnityEngine;

public abstract class ItemEffect : ScriptableObject, IInteractionEffect
{
    [SerializeField] private GameObject _effectPrefab;
    [SerializeField] private AudioClip _useSound;
    [Range(0f, 1f)][SerializeField] private float _useSoundVolume = 1f;

    public abstract void Execute();

    public void Use()
    {
        var player = IServiceLocator.Instance.GetService<IPlayerService>();
        if (_useSound != null && player != null)
            IServiceLocator.Instance.GetService<ISoundService>()
                ?.PlayOneShot(_useSound, player.Transform.position, _useSoundVolume);
        Execute();
    }

    protected void SpawnEffect()
    {
        if (_effectPrefab == null) return;

        var player = IServiceLocator.Instance.GetService<IPlayerService>();
        if (player == null) return;

        Object.Instantiate(_effectPrefab, player.Transform.position,
                           Quaternion.identity, player.Transform);
    }
}
