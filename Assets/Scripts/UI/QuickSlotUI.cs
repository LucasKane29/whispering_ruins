using UnityEngine;

public class QuickSlotUI : MonoBehaviour
{
    [SerializeField] private QuickSlotSlotUI[] _slots;

    private IQuickSlotService _service;

    private void Start()
    {
        _service = IServiceLocator.Instance.GetService<IQuickSlotService>();
        if (_service == null) return;

        _service.OnQuickSlotsChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (_service != null)
            _service.OnQuickSlotsChanged -= Refresh;
    }

    private void Refresh()
    {
        for (int i = 0; i < _slots.Length; i++)
            _slots[i].Setup(_service.GetSlot(i), i + 1);
    }
}
