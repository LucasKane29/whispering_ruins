using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text _label;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _hoverColor = Color.yellow;

    public void OnPointerEnter(PointerEventData _) => _label.color = _hoverColor;
    public void OnPointerExit(PointerEventData _) => _label.color = _normalColor;
}
