using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;

// Stub types to replace Godot-dependent types for testing
// These match the signatures of the actual types without Godot dependencies

namespace AlienInvasionLogistics.Source.UI.MenuControllers
{
    public enum StarType
    {
        MainSequence,
        BlueGiant,
        RedDwarf
    }

    public enum StartingResourcesMultiplier
    {
        Low,
        Medium,
        High
    }

    public struct GameSettings
    {
        public int NumberOfPlanets;
        public StarType StarType;
        public Guid PlayerNationId;
        public int StartingMineralModifier;
        public int StartingEnergyModifier;
        public int AiPlayerCount;
        public string PlayerName;
        public string SessionName;
    }
}

namespace AlienInvasionLogistics.Source.Types
{
    public enum ResourceTypes
    {
        Energy,
        Minerals
    }

    public class StartingResources
    {
        public int minerals;
        public int energy;
    }

    public class StartingResearch
    {
        public List<ResearchItem>? startingResearch;
    }
}

namespace AlienInvasionLogistics.Source.Interfaces
{
    public interface IGameResource
    {
        string Name { get; set; }
    }

    public interface IStellarBody
    {
        string DisplayName { get; set; }
        float Mass { get; set; }
    }
}

namespace AlienInvasionLogistics.Source.Database.Dtos
{
    public interface IGameObjectDto
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
