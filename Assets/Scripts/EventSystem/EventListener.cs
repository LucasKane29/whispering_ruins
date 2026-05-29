using UnityEngine;
using UnityEngine.Events;

public abstract class EventListener<T> : MonoBehaviour 
{ 
    [SerializeField] EventChannel<T> _eventChannel;
    [SerializeField] UnityEvent<T> _unityEvent;

    private void Awake()
    {
        _eventChannel.Register(this);
    }

    private void OnDestroy()
    {
        _eventChannel.Unregister(this);
    }

    public void Raise(T value)
    {
        _unityEvent?.Invoke(value);
    }
}

public class EventListener : EventListener<Empty>{}