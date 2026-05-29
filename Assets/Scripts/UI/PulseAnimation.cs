using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float amount = 5f;

    private RectTransform rt;
    private Vector2 basePosition;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        basePosition = rt.anchoredPosition;
    }

    private void Update()
    {
        rt.anchoredPosition = basePosition + Vector2.left * Mathf.Sin(Time.time * speed) * amount;
    }
}