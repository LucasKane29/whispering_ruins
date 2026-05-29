using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public GameObject prefab;
    public bool isConsumable = true;
    public bool isStackable = false;
    public ItemEffect effect;
}
