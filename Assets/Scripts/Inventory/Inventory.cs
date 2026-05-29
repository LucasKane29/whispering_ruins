using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        public ItemData item;
        public int count = 1;
    }

    [SerializeField] private List<Slot> _slots = new();

    public IReadOnlyCollection<Slot> Slots => _slots;

    public event Action OnInventoryChanged;
    public event Action<ItemData> OnItemAdded;
    public event Action<ItemData> OnItemRemoved;

    public bool Has(ItemData item, int amount = 1)
    {
        if (item == null)
            return false;
        int total = 0;
        foreach (Slot slot in _slots)
        {
            if (slot.item == item)
                total += slot.count;
        }
        return total >= amount;
    }

    public void Add(ItemData item, int amount = 1)
    {
        if(item == null) return;

        if (item.isStackable)
        {
            var existing = _slots.Find(slot => slot.item == item);
            if (existing != null)
                existing.count += amount;
            else
                _slots.Add(new Slot { item = item, count = 1 });
        }
        else
        {
            for (int i = 0; i < amount; i++)
                _slots.Add(new Slot { item = item, count = 1 });
        }
        OnItemAdded?.Invoke(item);
        OnInventoryChanged?.Invoke();
    }

    public bool Remove(ItemData item, int amount = 1)
    {
        if(Has(item, amount)) 
            return false;
        int left = amount;
        for(int i = _slots.Count - 1; i>= 0 && left > 0; i--)
        {
            if(_slots[i].item != item)
                continue;
            int take = Mathf.Min(_slots[i].count, left);
            _slots[i].count -= take;
            left -= take;
            if (_slots[i].count <= 0) _slots.RemoveAt(i);
        }
        OnItemRemoved?.Invoke(item);
        OnInventoryChanged?.Invoke();
        return true;
    }
}
