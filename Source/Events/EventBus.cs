using Godot;
using System;
using System.Collections.Generic;

namespace Core.Events
{
    /// <summary>
    /// Global event bus implementation for publish-subscribe pattern.
    /// Thread-safe for concurrent Subscribe/Unsubscribe/Publish calls.
    /// Handlers are invoked synchronously on the publishing thread.
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

        /// <summary>
        /// Subscribe a handler to events of type T. Duplicate subscriptions are ignored.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to</typeparam>
        /// <param name="handler">The callback to invoke when event is published</param>
        /// <exception cref="ArgumentNullException">Thrown when handler is null</exception>
        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock (_lock)
            {
                bool exists = DoesSubscriptionExist(handler);

                if (!exists)
                {
                    var eventType = typeof(T);

                    if (!_subscriptions.ContainsKey(eventType))
                    {
                        _subscriptions[eventType] = new List<Delegate>();
                    }

                    _subscriptions[eventType].Add(handler);
#if DEBUG
                    GD.Print(
                        $"Subscribed to {eventType.Name}. Total subscribers: {_subscriptions[eventType].Count}"
                    );
#endif
                }
                else
                {
                    GD.PushWarning(
                        $"Handler already subscribed to {typeof(T).Name}, skipping duplicate subscription"
                    );
                }
            }
        }

        /// <summary>
        /// Unsubscribe a handler from events of type T
        /// </summary>
        /// <typeparam name="T">The type of event to unsubscribe from</typeparam>
        /// <param name="handler">The handler to remove</param>
        /// <exception cref="ArgumentNullException">Thrown when handler is null</exception>
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
#if DEBUG
                    GD.Print(
                        $"Unsubscribed from {eventType.Name}. Remaining subscribers: {_subscriptions[eventType].Count}"
                    );
#endif

                    // Clean up empty lists
                    if (_subscriptions[eventType].Count == 0)
                    {
                        _subscriptions.Remove(eventType);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event to all subscribed handlers. Exceptions in handlers are caught and logged.
        /// </summary>
        /// <typeparam name="T">The type of event being published</typeparam>
        /// <param name="event">The event instance to publish</param>
        /// <exception cref="ArgumentNullException">Thrown when event is null</exception>
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
                handlers = [.. _subscriptions[eventType]];
            }

#if DEBUG
            GD.Print($"Publishing {eventType.Name} to {handlers.Count} subscriber(s)");
#endif

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

        /// <summary>
        /// Clear all subscriptions for a specific event type
        /// </summary>
        /// <typeparam name="T">The event type to clear subscriptions for</typeparam>
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

        /// <summary>
        /// Clear all subscriptions for all event types
        /// </summary>
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

        /// <summary>
        /// Check if a handler is already subscribed to events of type T.
        /// Note: Must be called within a lock on _lock
        /// </summary>
        /// <typeparam name="T">The event type to check</typeparam>
        /// <param name="handler">The handler to check for</param>
        /// <returns>True if the handler is already subscribed, false otherwise</returns>
        private bool DoesSubscriptionExist<T>(Action<T> handler) where T : IGameEvent
        {
            var eventType = typeof(T);

            if (_subscriptions.ContainsKey(eventType))
            {
                return _subscriptions[eventType].Contains(handler);
            }

            return false;
        }
    }
}
