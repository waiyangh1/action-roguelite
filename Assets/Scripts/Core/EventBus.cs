using System;
using System.Collections.Generic;

// The base interface
public interface IEvent { }

// The static messenger
public static class EventBus
{
    private static readonly Dictionary<Type, List<Action<IEvent>>> Subscribers = new();

    public static void Subscribe<T>(Action<IEvent> handler) where T : IEvent
    {
        var type = typeof(T);
        if (!Subscribers.ContainsKey(type)) Subscribers[type] = new List<Action<IEvent>>();
        Subscribers[type].Add(handler);
    }

    public static void Unsubscribe<T>(Action<IEvent> handler) where T : IEvent
    {
        var type = typeof(T);
        if (Subscribers.ContainsKey(type)) Subscribers[type].Remove(handler);
    }

    public static void Publish(IEvent gameEvent)
    {
        var type = gameEvent.GetType();
        if (Subscribers.ContainsKey(type))
        {
            foreach (var handler in Subscribers[type]) handler(gameEvent);
        }
    }
}