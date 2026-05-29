using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Image _healthLevel;
    private Health _health;

    private void Awake()
    {
        _health = GetComponentInParent<Health>();
        _health.OnHealthChanged += HandleHealthChanged;
    }

    public void HandleHealthChanged(float percentage)
    {
        _healthLevel.fillAmount = percentage;
    }
}
