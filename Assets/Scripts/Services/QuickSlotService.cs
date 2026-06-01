using System;
using System.Linq;

public class QuickSlotService : IQuickSlotService
{
    public const int SlotCount = 4;
    int IQuickSlotService.SlotCount => SlotCount;

    private readonly IInventoryService _inventory;

    public event Action OnQuickSlotsChanged;

    public QuickSlotService(IInventoryService inventory)
    {
        _inventory = inventory;
        _inventory.OnInventoryChanged += RaiseChanged;
    }

    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= SlotCount) return null;
        var usable = _inventory.Slots.Where(s => s.Item.effect != null)
                                     .ToList();
        return index < usable.Count ? usable[index] : null;
    }

    public bool Use(int index)
    {
        var slot = GetSlot(index);
        if (slot?.Item == null) return false;

        slot.Item.effect?.Use();

        if (slot.Item.isConsumable)
            _inventory.Remove(slot.Item, 1);

        return true;
    }

    private void RaiseChanged() => OnQuickSlotsChanged?.Invoke();
}
