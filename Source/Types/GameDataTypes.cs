using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Interfaces;

namespace AlienInvasionLogistics.Source.Types;

public enum GameDifficulty
{
    Easy,
    Medium,
    Hard
}

public interface IGameObject {}

public interface IGameObjectDto {}

public interface IBlueprint {}

public class StartingSettings : IStartingSettings
{
    public string GameName { get; set; }
    public GameDifficulty Difficulty { get; set; }
    public int CelestialBodyCount { get; set; }
    public int AiPlayerCount { get; set; }
    public Star CentralSolarMass { get; set; }
}