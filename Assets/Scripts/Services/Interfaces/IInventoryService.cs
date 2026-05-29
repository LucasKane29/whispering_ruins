using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData Item;
    public int count = 1;
}

public interface IInventoryService: IService
{
    IReadOnlyList<InventorySlot> Slots { get; }

    event Action OnInventoryChanged;
    event Action<ItemData> OnItemAdded;
    event Action<ItemData> OnItemRemoved;

    bool Has(ItemData item, int amount = 1);
    void Add(ItemData item, int amount = 1);
    bool Remove(ItemData item, int amount = 1);
    void Clear();
}
