using UnityEngine;

[CreateAssetMenu(fileName = "StaminaPotionEffect", menuName = "Inventory/Effects/Stamina Potion")]
public class StaminaPotionEffect : ItemEffect
{
    [SerializeField] private float _restoreAmount = 50f;

    public override void Execute()
    {
        var playerService = IServiceLocator.Instance.GetService<IPlayerService>();
        if (playerService == null) return;
        SpawnEffect();
        playerService.Stamina.Restore(_restoreAmount);
    }
}
