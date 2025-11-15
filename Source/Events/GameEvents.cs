using System;
using Core.Database.Models;

namespace Core.Events
{
    // ==================== Game Lifecycle Events ====================

    /// <summary>
    /// Published when a new game is created
    /// </summary>
    public class GameCreatedEvent : GameEvent
    {
        public Guid GameDataId { get; }
        public string PlayerName { get; }

        public GameCreatedEvent(Guid gameDataId, string playerName)
        {
            GameDataId = gameDataId;
            PlayerName = playerName;
        }
    }

    /// <summary>
    /// Published when a game is loaded from save
    /// </summary>
    public class GameLoadedEvent : GameEvent
    {
        public Guid GameDataId { get; }
        public string SaveName { get; }

        public GameLoadedEvent(Guid gameDataId, string saveName)
        {
            GameDataId = gameDataId;
            SaveName = saveName;
        }
    }

    /// <summary>
    /// Published when a game is saved
    /// </summary>
    public class GameSavedEvent : GameEvent
    {
        public Guid SaveId { get; }
        public string SaveName { get; }

        public GameSavedEvent(Guid saveId, string saveName)
        {
            SaveId = saveId;
            SaveName = saveName;
        }
    }

    // ==================== Resource Events ====================

    /// <summary>
    /// Published when player resources change
    /// </summary>
    public class ResourcesChangedEvent : GameEvent
    {
        public int EnergyStored { get; }
        public int MineralsStored { get; }
        public float EnergyIncomeDaily { get; }
        public float MineralsIncomeDaily { get; }

        public ResourcesChangedEvent(
            int energyStored,
            int mineralsStored,
            float energyIncome,
            float mineralsIncome
        )
        {
            EnergyStored = energyStored;
            MineralsStored = mineralsStored;
            EnergyIncomeDaily = energyIncome;
            MineralsIncomeDaily = mineralsIncome;
        }
    }

    // ==================== Research Events ====================

    /// <summary>
    /// Published when research is started
    /// </summary>
    public class ResearchStartedEvent : GameEvent
    {
        public Guid ResearchId { get; }
        public string ResearchName { get; }

        public ResearchStartedEvent(Guid researchId, string researchName)
        {
            ResearchId = researchId;
            ResearchName = researchName;
        }
    }

    /// <summary>
    /// Published when research is completed
    /// </summary>
    public class ResearchCompletedEvent : GameEvent
    {
        public Guid ResearchId { get; }
        public string ResearchName { get; }

        public ResearchCompletedEvent(Guid researchId, string researchName)
        {
            ResearchId = researchId;
            ResearchName = researchName;
        }
    }

    // ==================== Time Events ====================

    /// <summary>
    /// Published when game time advances (daily tick)
    /// </summary>
    public class DayAdvancedEvent : GameEvent
    {
        public int CurrentDay { get; }

        public DayAdvancedEvent(int currentDay)
        {
            CurrentDay = currentDay;
        }
    }

    /// <summary>
    /// Published when time acceleration changes
    /// </summary>
    public class TimeAccelerationChangedEvent : GameEvent
    {
        public float NewAcceleration { get; }
        public float OldAcceleration { get; }

        public TimeAccelerationChangedEvent(float newAcceleration, float oldAcceleration)
        {
            NewAcceleration = newAcceleration;
            OldAcceleration = oldAcceleration;
        }
    }

    // ==================== UI Events ====================

    /// <summary>
    /// Published when a screen/scene needs to be changed
    /// </summary>
    public class SceneChangeRequestedEvent : GameEvent
    {
        public string ScenePath { get; }

        public SceneChangeRequestedEvent(string scenePath)
        {
            ScenePath = scenePath;
        }
    }

    /// <summary>
    /// Published when UI needs to be refreshed
    /// </summary>
    public class UIRefreshRequestedEvent : GameEvent
    {
        public string UIElementName { get; }

        public UIRefreshRequestedEvent(string uiElementName = null)
        {
            UIElementName = uiElementName;
        }
    }

    // ==================== Error Events ====================

    /// <summary>
    /// Published when an error occurs
    /// </summary>
    public class ErrorOccurredEvent : GameEvent
    {
        public string Message { get; }
        public Exception Exception { get; }
        public string Severity { get; }

        public ErrorOccurredEvent(
            string message,
            Exception exception = null,
            string severity = "Error"
        )
        {
            Message = message;
            Exception = exception;
            Severity = severity;
        }
    }
}
