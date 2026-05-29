using UnityEngine;

public class BossAOEEffect : MonoBehaviour
{
    [SerializeField] private float _lifetime = 2f;
    [SerializeField] private float _tickInterval = 0.5f;

    private float _damage;
    private float _tickTimer;

    public void Init(float damage)
    {
        _damage = damage;
        _tickTimer = 0f;
        Destroy(gameObject, _lifetime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _tickTimer -= Time.deltaTime;
        if (_tickTimer <= 0f)
        {
            _tickTimer = _tickInterval;
            other.GetComponent<Health>()?.TakeDamage(_damage);
        }
    }
}
