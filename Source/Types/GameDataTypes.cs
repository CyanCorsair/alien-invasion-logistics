using System;
using System.Collections.Generic;

namespace Core.Types;

public struct PlayerStateData
{
    public Guid StateId;
}

public struct SolarSystemStateData
{
    public Guid StateId;
}

public struct GameSaveData
{
    public string SaveName;
    public DateTime SaveTime;
    public Guid GameDataId;
    public int InGameDay;
    public int GameMode; // 0 = Strat, 1 = Tac
}

public struct GameStateData
{
    public Guid StateId;
    public int CurrentInGameDay;
    public float CurrentTimeAcceleration;
    public int GameMode; // 0 = Strat, 1 = Tac
    public List<Guid> AIPlayerStateIds;
    public Guid PlayerStateId;
    public PlayerStateData HumanPlayerState;
    public List<PlayerStateData> AIPlayerStates;
    public Guid SolarSystemStateId;
    public SolarSystemStateData SolarSystemState;
}
