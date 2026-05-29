using UnityEngine;

public abstract class ItemEffect : ScriptableObject, IInteractionEffect
{
    [SerializeField] private GameObject _effectPrefab;

    public abstract void Execute();

    protected void SpawnEffect()
    {
        if (_effectPrefab == null) return;

        var player = IServiceLocator.Instance.GetService<IPlayerService>();
        if (player == null) return;

        Object.Instantiate(_effectPrefab, player.Transform.position,
                           Quaternion.identity, player.Transform);
    }
}
