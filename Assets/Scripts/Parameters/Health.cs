using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _invincibilityDuration = 0f;
    [SerializeField] FloatEventChannel _channel;
    public Action<float> OnHealthChanged;
    public Action OnDeath;
    public Action OnDamaged;
    private float _currentHealth;
    private float _invincibilityTimer;
    private bool _initialized;

    public bool IsDead => _currentHealth <= 0;
    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;

    void Awake()
    {
        if (!_initialized)
            _currentHealth = _maxHealth;
    }

    void Update()
    {
        if (_invincibilityTimer > 0f)
            _invincibilityTimer -= Time.deltaTime;
    }

    private void Start()
    {
        PublishHealthPercentage();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);
        PublishHealthPercentage();
    }

    public void TakeDamage(float damage, bool triggerHurt = true)
    {
        if (IsDead) return;
        if (triggerHurt && _invincibilityTimer > 0f) return;

        if (triggerHurt && _invincibilityDuration > 0f)
            _invincibilityTimer = _invincibilityDuration;

        _currentHealth -= damage;

        OnHealthChanged?.Invoke(_currentHealth / _maxHealth);

        if (IsDead)
        {
            _currentHealth = Mathf.Max(0, _currentHealth);
            OnDeath?.Invoke();
        }
        else if (triggerHurt)
        {
            OnDamaged?.Invoke();
        }

        PublishHealthPercentage();
    }

    public void Initialize(float value)
    {
        _initialized = true;
        _currentHealth = Mathf.Clamp(value, 0f, _maxHealth);
        PublishHealthPercentage();
    }

    private void PublishHealthPercentage()
    {
        if(_channel != null)
            _channel.Invoke(_currentHealth / _maxHealth);
    }
}
