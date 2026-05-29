using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServicesBootstrap : MonoBehaviour
{
    void Awake()
    {
        var inventoryService = new InventoryService();
        IServiceLocator.Instance.TryRegisterService<IInventoryService>(inventoryService);
        IServiceLocator.Instance.TryRegisterService<IQuickSlotService>(new QuickSlotService(inventoryService));
        IServiceLocator.Instance.TryRegisterService<IService>(new LogService());
    }

    void OnDestroy()
    {
        IServiceLocator.Instance.TryUnregisterService<IInventoryService>(
            IServiceLocator.Instance.GetService<IInventoryService>());
        IServiceLocator.Instance.TryUnregisterService<IQuickSlotService>(
            IServiceLocator.Instance.GetService<IQuickSlotService>());
        IServiceLocator.Instance.TryUnregisterService<IService>(new LogService());
    }
}
