using System;
using UnityEngine;

public class Stamina : MonoBehaviour
{
    [SerializeField] private float _maxStamina = 100f;
    [SerializeField] private float _regenDelay = 2f;
    [SerializeField] private float _regenRate = 20f;
    [SerializeField] FloatEventChannel _channel;
    [SerializeField] private float _exhaustedStaminaRegenCooldown = 5f;
    public Action<float> OnStaminaChanged;
    private float _currentStamina;
    private StopwatchTimer _regenTimer;
    private bool _isExhausted = false;
    private bool _initialized;

    public float CurrentStamina => _currentStamina;

    public bool HasEnoughStamina(float amount) => _currentStamina >= amount;

    void Awake()
    {

    }

    private void Start()
    {
        _regenTimer = new StopwatchTimer();
        if (!_initialized)
            _currentStamina = _maxStamina;
        PublishStaminaPercentage();
    }

    public void Update()
    {
        _regenTimer.Tick(Time.deltaTime);
        float regenDelay = _isExhausted ? _exhaustedStaminaRegenCooldown : _regenDelay;
        if (_regenTimer.IsRunning && _regenTimer.GetTime() >= regenDelay)
        {
            _isExhausted = false;
            _currentStamina = Mathf.Min(_maxStamina, _currentStamina + _regenRate * Time.deltaTime);
            OnStaminaChanged?.Invoke(_currentStamina / _maxStamina);
            PublishStaminaPercentage();
        }
    }

    public void Restore(float amount)
    {
        _currentStamina = Mathf.Min(_currentStamina + amount, _maxStamina);
        _isExhausted = false;
        _regenTimer.Reset();
        OnStaminaChanged?.Invoke(_currentStamina / _maxStamina);
        PublishStaminaPercentage();
    }

    public void UseStamina(float amount)
    {
        if (_isExhausted)
            return;
        _currentStamina -= amount;
        _currentStamina = Mathf.Max(0, _currentStamina);
        if (_currentStamina == 0)
        {
            _isExhausted = true;
        }
        OnStaminaChanged?.Invoke(_currentStamina / _maxStamina);

        _regenTimer.Reset();
        _regenTimer.Start();
        PublishStaminaPercentage();
    }   

    public void Initialize(float value)
    {
        _initialized = true;
        _currentStamina = Mathf.Clamp(value, 0f, _maxStamina);
        _isExhausted = false;
        _regenTimer?.Stop();   // stop before reset so regen doesn't resume after regenDelay
        _regenTimer?.Reset();
        PublishStaminaPercentage();
    }

    private void PublishStaminaPercentage()
    {
        if (_channel != null)
            _channel.Invoke(_currentStamina / _maxStamina);
    }
}
