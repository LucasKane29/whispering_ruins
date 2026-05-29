using System;
using System.Collections.Generic;

public class InventoryService : IInventoryService
{
    private readonly List<InventorySlot> slots = new();
    public IReadOnlyList<InventorySlot> Slots => slots;

    public event Action OnInventoryChanged;
    public event Action<ItemData> OnItemAdded;
    public event Action<ItemData> OnItemRemoved;

    public bool Has(ItemData item, int amount = 1)
    {
        if (item == null) return false;
        int total = 0;
        foreach (var s in slots)
            if (s.Item == item) total += s.count;
        return total >= amount;
    }

    public void Add(ItemData item, int amount = 1)
    {
        if (item == null) return;

        if (item.isStackable)
        {
            var existing = slots.Find(s => s.Item == item);
            if (existing != null) existing.count += amount;
            else slots.Add(new InventorySlot { Item = item, count = amount });
        }
        else
        {
            for (int i = 0; i < amount; i++)
                slots.Add(new InventorySlot { Item = item, count = 1 });
        }

        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
    }

    public bool Remove(ItemData item, int amount = 1)
    {
        if (!Has(item, amount)) return false;

        int left = amount;
        for (int i = slots.Count - 1; i >= 0 && left > 0; i--)
        {
            if (slots[i].Item != item) continue;
            int take = Math.Min(slots[i].count, left);
            slots[i].count -= take;
            left -= take;
            if (slots[i].count <= 0) slots.RemoveAt(i);
        }

        OnItemRemoved?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void Clear()
    {
        slots.Clear();
        OnInventoryChanged?.Invoke();
    }
}