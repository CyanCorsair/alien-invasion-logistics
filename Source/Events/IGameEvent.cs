using System;

namespace Core.Events
{
    /// <summary>
    /// Base interface for all game events
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// Timestamp when the event was created
        /// </summary>
        DateTime Timestamp { get; }
    }

    /// <summary>
    /// Base class for game events with timestamp tracking
    /// </summary>
    public abstract class GameEvent : IGameEvent
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
