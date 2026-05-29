using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraService : MonoBehaviour, ICameraService
{
    public Transform Transform => transform;
    public Camera Camera => GetComponent<Camera>();

    private void Awake()
    {
        IServiceLocator.Instance.TryRegisterService<ICameraService, CameraService>(this);
    }

    private void OnDestroy()
    {
        if (IServiceLocator.Instance != null)
        {
            IServiceLocator.Instance.TryUnregisterService<ICameraService, CameraService>(this);
        }
    }
}
