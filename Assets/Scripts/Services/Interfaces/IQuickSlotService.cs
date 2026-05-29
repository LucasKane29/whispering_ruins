using System;

public interface IQuickSlotService : IService
{
    int SlotCount { get; }
    InventorySlot GetSlot(int index);
    bool Use(int index);
    event Action OnQuickSlotsChanged;
}
