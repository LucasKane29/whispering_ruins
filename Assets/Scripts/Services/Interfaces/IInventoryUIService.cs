using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryUIService : IService
{
    void Open();
    void OpenForSelection(string prompt, Action<ItemData> onPicked, Action onCancel);

    void Close();
    bool IsOpen { get; }
}
