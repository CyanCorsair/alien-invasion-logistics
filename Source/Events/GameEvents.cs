using System;
using AlienInvasionLogistics.Source.Utilities;

namespace AlienInvasionLogistics.Source.Events;
// ==================== Game Lifecycle Events ====================

/// <summary>
///     Published when a new game is created
/// </summary>
public class GameCreatedEvent : GameEvent
{
    public GameCreatedEvent(Guid gameDataId, string playerName)
    {
        GameDataId = gameDataId;
        PlayerName = playerName;
    }

    public Guid GameDataId { get; }
    public string PlayerName { get; }
}

/// <summary>
///     Published when a game is loaded from save
/// </summary>
public class GameLoadedEvent : GameEvent
{
    public GameLoadedEvent(Guid gameDataId, string saveName)
    {
        GameDataId = gameDataId;
        SaveName = saveName;
    }

    public Guid GameDataId { get; }
    public string SaveName { get; }
}

/// <summary>
///     Published when a game is saved
/// </summary>
public class GameSavedEvent : GameEvent
{
    public GameSavedEvent(Guid saveId, string saveName)
    {
        SaveId = saveId;
        SaveName = saveName;
    }

    public Guid SaveId { get; }
    public string SaveName { get; }
}

// ==================== Resource Events ====================

/// <summary>
///     Published when player resources change
/// </summary>
public class ResourcesChangedEvent : GameEvent
{
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

    public int EnergyStored { get; }
    public int MineralsStored { get; }
    public float EnergyIncomeDaily { get; }
    public float MineralsIncomeDaily { get; }
}

// ==================== Research Events ====================

/// <summary>
///     Published when research is started
/// </summary>
public class ResearchStartedEvent : GameEvent
{
    public ResearchStartedEvent(Guid researchId, string researchName)
    {
        ResearchId = researchId;
        ResearchName = researchName;
    }

    public Guid ResearchId { get; }
    public string ResearchName { get; }
}

/// <summary>
///     Published when research is completed
/// </summary>
public class ResearchCompletedEvent : GameEvent
{
    public ResearchCompletedEvent(Guid researchId, string researchName)
    {
        ResearchId = researchId;
        ResearchName = researchName;
    }

    public Guid ResearchId { get; }
    public string ResearchName { get; }
}

// ==================== Time Events ====================

/// <summary>
///     Published when game time advances (daily tick)
/// </summary>
public class DayAdvancedEvent : GameEvent
{
    public DayAdvancedEvent(int currentDay)
    {
        CurrentDay = currentDay;
    }

    public int CurrentDay { get; }
}

/// <summary>
///     Published when time acceleration changes
/// </summary>
public class TimeAccelerationChangedEvent : GameEvent
{
    public TimeAccelerationChangedEvent(float newAcceleration, float oldAcceleration)
    {
        NewAcceleration = newAcceleration;
        OldAcceleration = oldAcceleration;
    }

    public float NewAcceleration { get; }
    public float OldAcceleration { get; }
}

// ==================== UI Events ====================

/// <summary>
///     Published when a screen/scene needs to be changed
/// </summary>
public class SceneChangeRequestedEvent : GameEvent
{
    public SceneChangeRequestedEvent(string scenePath)
    {
        ScenePath = scenePath;
    }

    public string ScenePath { get; }
}

/// <summary>
///     Published when UI needs to be refreshed
/// </summary>
public class UiRefreshRequestedEvent : GameEvent
{
    public UiRefreshRequestedEvent(string uiElementName = null)
    {
        UiElementName = uiElementName;
    }

    public string UiElementName { get; }
}

/// <summary>
///     Published when a solar system is generated
/// </summary>
public class SolarSystemGeneratedEvent : GameEvent
{
    public SolarSystemGeneratedEvent(Guid solarSystemId, int planetCount)
    {
        SolarSystemId = solarSystemId;
        PlanetCount = planetCount;
    }

    public Guid SolarSystemId { get; }
    public int PlanetCount { get; }
}

// ==================== Error Events ====================

/// <summary>
///     Published when an error occurs
/// </summary>
public class ErrorOccurredEvent : GameEvent
{
    public ErrorOccurredEvent(
        string message,
        Exception exception = null,
        ErrorUtilities.MessageLevel severity = ErrorUtilities.MessageLevel.Error
    )
    {
        Message = message;
        Exception = exception;
        Severity = severity;
    }

    public string Message { get; }
    public Exception Exception { get; }
    public ErrorUtilities.MessageLevel Severity { get; }
}