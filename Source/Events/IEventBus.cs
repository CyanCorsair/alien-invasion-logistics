using System;

namespace Core.Events
{
    /// <summary>
    /// Global event bus for loosely coupled communication between game systems
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribe to events of type T
        /// </summary>
        /// <typeparam name="T">Event type to subscribe to</typeparam>
        /// <param name="handler">Callback to invoke when event is published</param>
        void Subscribe<T>(Action<T> handler) where T : IGameEvent;

        /// <summary>
        /// Unsubscribe from events of type T
        /// </summary>
        /// <typeparam name="T">Event type to unsubscribe from</typeparam>
        /// <param name="handler">Callback to remove</param>
        void Unsubscribe<T>(Action<T> handler) where T : IGameEvent;

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <typeparam name="T">Event type</typeparam>
        /// <param name="event">Event instance to publish</param>
        void Publish<T>(T @event) where T : IGameEvent;

        /// <summary>
        /// Clear all subscriptions for a specific event type
        /// </summary>
        /// <typeparam name="T">Event type to clear</typeparam>
        void ClearSubscriptions<T>() where T : IGameEvent;

        /// <summary>
        /// Clear all subscriptions for all event types
        /// </summary>
        void ClearAllSubscriptions();
    }
}
