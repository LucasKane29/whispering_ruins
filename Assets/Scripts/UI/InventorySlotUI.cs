using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _countLabel;
    [SerializeField] private Button _button;

    private ItemData _item;
    private InventoryUIService _inventoryUIService;

    void Start()
    {
        _inventoryUIService = IServiceLocator.Instance.GetService<IInventoryUIService>() as InventoryUIService; 
    }

    public void Setup(ItemData item, int count)
    {
        _item = item;
        _iconImage.sprite = item.icon;
        _countLabel.text = count > 1 ? count.ToString() : string.Empty;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => _inventoryUIService.HandleSlotClicked(_item));
    }
}
