using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemData> _items = new();

    public ItemData GetById(string id) => _items.Find(i => i.id == id);
}
