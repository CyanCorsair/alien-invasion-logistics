using Godot;
using System;
using System.Collections.Generic;

namespace Core.Events
{
    /// <summary>
    /// Global event bus implementation for publish-subscribe pattern
    /// </summary>
    [GlobalClass]
    public partial class EventBus : Node, IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();
        private readonly object _lock = new();

        public override void _Ready()
        {
            GD.Print("EventBus initialized");
        }

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                var eventType = typeof(T);

                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = new List<Delegate>();
                }

                _subscriptions[eventType].Add(handler);
                GD.Print(
                    $"Subscribed to {eventType.Name}. Total subscribers: {_subscriptions[eventType].Count}"
                );
            }
        }

        public void Unsubscribe<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                var eventType = typeof(T);

                if (_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType].Remove(handler);
                    GD.Print(
                        $"Unsubscribed from {eventType.Name}. Remaining subscribers: {_subscriptions[eventType].Count}"
                    );

                    // Clean up empty lists
                    if (_subscriptions[eventType].Count == 0)
                    {
                        _subscriptions.Remove(eventType);
                    }
                }
            }
        }

        public void Publish<T>(T @event) where T : IGameEvent
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }

            List<Delegate> handlers;
            var eventType = typeof(T);

            lock (_lock)
            {
                if (!_subscriptions.ContainsKey(eventType))
                {
                    // No subscribers for this event type
                    return;
                }

                // Create a copy to avoid issues with handlers that modify subscriptions
                handlers = new List<Delegate>(_subscriptions[eventType]);
            }

            GD.Print($"Publishing {eventType.Name} to {handlers.Count} subscriber(s)");

            foreach (var handler in handlers)
            {
                try
                {
                    ((Action<T>)handler).Invoke(@event);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"Error invoking event handler for {eventType.Name}: {ex.Message}");
                    GD.PrintErr(ex.StackTrace);
                }
            }
        }

        public void ClearSubscriptions<T>() where T : IGameEvent
        {
            lock (_lock)
            {
                var eventType = typeof(T);

                if (_subscriptions.ContainsKey(eventType))
                {
                    var count = _subscriptions[eventType].Count;
                    _subscriptions.Remove(eventType);
                    GD.Print($"Cleared {count} subscription(s) for {eventType.Name}");
                }
            }
        }

        public void ClearAllSubscriptions()
        {
            lock (_lock)
            {
                var totalSubscriptions = 0;
                foreach (var kvp in _subscriptions)
                {
                    totalSubscriptions += kvp.Value.Count;
                }

                _subscriptions.Clear();
                GD.Print($"Cleared all subscriptions ({totalSubscriptions} total)");
            }
        }

        public override void _ExitTree()
        {
            ClearAllSubscriptions();
        }
    }
}
