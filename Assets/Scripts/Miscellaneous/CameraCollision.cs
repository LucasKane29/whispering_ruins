using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraCollision : MonoBehaviour
{
    [SerializeField] private float _collisionRadius = 0.2f;
    [SerializeField] private float _returnSpeed = 5f;
    [SerializeField] private LayerMask _collisionLayers;

    private CinemachineVirtualCamera _vcam;
    private CinemachineFramingTransposer _transposer;
    private float _defaultDistance;

    private void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
        _transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _defaultDistance = _transposer.m_CameraDistance;
    }

    private void LateUpdate()
    {
        if (_transposer == null || _vcam.Follow == null) return;

        Vector3 origin = _vcam.Follow.position;
        Vector3 direction = (transform.position - origin).normalized;

        if (Physics.SphereCast(origin, _collisionRadius, direction, out RaycastHit hit, _defaultDistance, _collisionLayers))
            _transposer.m_CameraDistance = Mathf.Max(hit.distance - _collisionRadius, 0.5f);
        else
            _transposer.m_CameraDistance = Mathf.Lerp(_transposer.m_CameraDistance, _defaultDistance, Time.deltaTime * _returnSpeed);
    }
}