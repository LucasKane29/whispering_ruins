using UnityEngine;

public class Billboard : MonoBehaviour
{
    private CameraService _cameraService;
    private Camera _camera;

    void Start()
    {
        _cameraService = IServiceLocator.Instance.GetService<CameraService>();
        _camera = _cameraService?.Transform.GetComponent<Camera>();
    }
    void Update()
    {
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
                 _camera.transform.rotation * Vector3.up);
    }
}
