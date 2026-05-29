using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _fillImage;
    [SerializeField] private Image _delayedFillImage;
    [SerializeField] private TextMeshProUGUI _bossNameText;
    [SerializeField] private float _fillLerpSpeed = 8f;
    [SerializeField] private float _delayedFillDelay = 0.5f;
    [SerializeField] private float _delayedFillSpeed = 3f;

    private BossController _boss;
    private float _targetFill = 1f;
    private float _delayedFillTimer;

    private void Start()
    {
        _panel.SetActive(false);
    }

    public void Init(BossController boss)
    {
        _boss = boss;
        _boss.OnBossActivated += Show;
        _boss.Health.OnHealthChanged += OnHealthChanged;
        _boss.Health.OnDeath += OnDeath;
        _bossNameText.SetText(_boss.BossName);
    }

    private void OnDestroy()
    {
        if (_boss == null) return;
        _boss.OnBossActivated -= Show;
        _boss.Health.OnHealthChanged -= OnHealthChanged;
        _boss.Health.OnDeath -= OnDeath;
    }

    private void Update()
    {
        if (!_panel.activeSelf) return;

        _fillImage.fillAmount = Mathf.Lerp(_fillImage.fillAmount, _targetFill, Time.deltaTime * _fillLerpSpeed);

        if (_delayedFillImage == null) return;
        _delayedFillTimer -= Time.deltaTime;
        if (_delayedFillTimer <= 0f)
            _delayedFillImage.fillAmount = Mathf.Lerp(_delayedFillImage.fillAmount, _targetFill, Time.deltaTime * _delayedFillSpeed);
    }

    private void Show()
    {
        _panel.SetActive(true);
        _targetFill = 1f;
        _fillImage.fillAmount = 1f;
        if (_delayedFillImage != null) _delayedFillImage.fillAmount = 1f;
    }

    private void OnHealthChanged(float percentage)
    {
        _targetFill = percentage;
        _delayedFillTimer = _delayedFillDelay;
    }

    private void OnDeath()
    {
        StartCoroutine(HideAfterDelay(2.5f));
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _panel.SetActive(false);
    }
}
