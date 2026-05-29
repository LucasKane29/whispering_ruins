using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotSlotUI : MonoBehaviour
{
    [SerializeField] private Image      _iconImage;
    [SerializeField] private TMP_Text   _countLabel;
    [SerializeField] private TMP_Text   _keyLabel;
    [SerializeField] private GameObject _emptyOverlay;

    public void Setup(InventorySlot slot, int keyNumber)
    {
        _keyLabel.text = keyNumber.ToString();

        bool hasItem = slot?.Item != null;
        _iconImage.gameObject.SetActive(hasItem);
        _emptyOverlay.SetActive(!hasItem);

        if (!hasItem)
        {
            _countLabel.text = string.Empty;
            return;
        }

        _iconImage.sprite  = slot.Item.icon;
        _countLabel.text   = slot.count > 1 ? slot.count.ToString() : string.Empty;
    }
}
