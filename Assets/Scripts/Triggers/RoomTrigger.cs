using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] private BaseSpawner _enemySpawner;
    private bool _isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isTriggered && other.CompareTag("Player"))
        {
            _isTriggered = true;
            _enemySpawner.Spawn();
        }
    }
}
