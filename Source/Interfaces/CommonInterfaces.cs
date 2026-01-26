using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using Godot;

namespace AlienInvasionLogistics.Source.Interfaces;

public interface IGameResource
{
    public string Name { get; set; }
    public Sprite2D Icon { get; set; }
}

public interface IBasePlayer
{
    public bool IsHuman { get; set; }
    public string PlayerName { get; set; }
    public Guid PlayerId { get; set; }
    public ResearchState Researches { get; set; }
    public ResourcesState Resources { get; set; }
}

public interface IStellarBody
{
    public string DisplayName { get; set; }
    public Vector2 Location2D { get; set; }
    public Vector2 Velocity2D { get; set; }
    public float Mass { get; set; }
}

public interface IPersistable
{
    public void Save();
    public void Load();
}

public interface IStartingSettings
{
    public string GameName { get; set; }
    public GameDifficulty Difficulty { get; set; }
    public int CelestialBodyCount { get; set; }
    public int AiPlayerCount { get; set; }
    public Star CentralSolarMass { get; set; }
}