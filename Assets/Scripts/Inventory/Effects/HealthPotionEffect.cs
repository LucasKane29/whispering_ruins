using UnityEngine;

[CreateAssetMenu(fileName = "HealthPotionEffect", menuName = "Inventory/Effects/Health Potion")]
public class HealthPotionEffect : ItemEffect
{
    [SerializeField] private float _healAmount = 50f;

    public override void Execute()
    {
        var playerService = IServiceLocator.Instance.GetService<IPlayerService>();
        if (playerService == null) return;
        SpawnEffect();
        playerService.Health.Heal(_healAmount);
    }
}
