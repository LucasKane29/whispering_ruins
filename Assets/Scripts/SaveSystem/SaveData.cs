using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string sceneSlot;
    public string sceneName;
    public float health;
    public float stamina;
    public int souls;
    public List<SavedInventorySlot> inventory = new();
    public List<string> clearedSpawners = new();
}

[Serializable]
public class SavedInventorySlot
{
    public string itemId;
    public int count;
}
