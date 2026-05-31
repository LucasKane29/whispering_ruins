using UnityEngine;
using UnityEngine.Events;

public class ItemSocketEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent _onInsertedIntoSocket;

    public void OnInserted() => _onInsertedIntoSocket?.Invoke();
}
