using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BossProjectile : MonoBehaviour
{
    [SerializeField] private float _lifetime = 8f;
    public GameObject ExplosionPrefab;
    public float DestroyExplosion = 4.0f;
    public float DestroyChildren = 2.0f;

    private float _damage;
    private float _ignoreTimer = 0.1f;
    private bool _isDestroying = false;

    private void Awake()
    {
        var rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Init(float speed, float damage, Vector3 direction)
    {
        _damage = damage;
        transform.rotation = Quaternion.LookRotation(direction);
        GetComponent<Rigidbody>().velocity = direction * speed;
        Destroy(gameObject, _lifetime);
    }

    private void Update()
    {
        if (_ignoreTimer > 0f)
            _ignoreTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_ignoreTimer > 0f && _isDestroying) return;
        _isDestroying = true;
        if (other.CompareTag("Player"))
            other.GetComponent<Health>()?.TakeDamage(_damage);

        var exp = Instantiate(ExplosionPrefab, transform.position, ExplosionPrefab.transform.rotation);
        Destroy(exp, DestroyExplosion);
        if(transform.childCount > 0)
        {
            Transform child = transform.GetChild(0);
            transform.DetachChildren();
            Destroy(child.gameObject, DestroyChildren);
        }
        Destroy(gameObject);
    }
}
