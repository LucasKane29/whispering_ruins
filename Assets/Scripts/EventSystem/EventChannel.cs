using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventChannel<T> : ScriptableObject
{
    public event Action<T> OnEventRaised;
    private readonly HashSet<EventListener<T>> _listeners = new ();

    public void Invoke(T value)
    {
        OnEventRaised?.Invoke(value);
        foreach (var listener in _listeners)
        {
            listener.Raise(value);
        }
    }

    public void Register(EventListener<T> listener)
    {
        _listeners.Add(listener);
    }

    public void Unregister(EventListener<T> listener)
    {
        _listeners.Remove(listener);
    }
}

public struct Empty { }

[CreateAssetMenu(menuName = "Events/EventChannel")]
public class EventChannel : EventChannel<Empty> { }