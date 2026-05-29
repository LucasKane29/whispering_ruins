using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
public class PlayerQuickSlot : MonoBehaviour
{
    private PlayerInputs _inputs;
    private IQuickSlotService _quickSlotService;

    private void Awake()
    {
        _inputs = GetComponent<PlayerInputs>();
    }

    private void Start()
    {
        _quickSlotService = IServiceLocator.Instance.GetService<IQuickSlotService>();
    }

    private void Update()
    {
        if (_quickSlotService == null) return;

        TryUse(ref _inputs.quickSlot1, 0);
        TryUse(ref _inputs.quickSlot2, 1);
        TryUse(ref _inputs.quickSlot3, 2);
        TryUse(ref _inputs.quickSlot4, 3);
    }

    private void TryUse(ref bool input, int slotIndex)
    {
        if (!input) return;
        input = false;
        _quickSlotService.Use(slotIndex);
    }
}
