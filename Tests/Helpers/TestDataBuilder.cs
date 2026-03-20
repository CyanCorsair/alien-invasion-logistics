using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Dtos;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.UI.MenuControllers;

namespace AlienInvasionLogistics.Tests.Helpers;

/// <summary>
///     Builder class for creating test data objects
/// </summary>
public static class TestDataBuilder
{
    // ==================== GameSession ====================

    public static GameSession CreateDefaultGameSession()
    {
        return new GameSession();
    }

    public static GameSession CreateGameSession(string sessionName, string playerName, int planets = 5)
    {
        return new GameSession
        {
            SessionName = sessionName,
            SaveName = $"{playerName} - {sessionName} - Day 0",
            PlayerName = playerName,
            NumberOfPlanets = planets,
            InGameDay = 0,
            CreatedAt = DateTime.UtcNow,
            StarType = StarType.MainSequence,
            NumberOfAiPlayers = 0,
            StartingMineralModifier = 1,
            StartingEnergyModifier = 1
        };
    }

    // ==================== Player ====================

    public static Player CreateDefaultPlayer()
    {
        return new Player();
    }

    public static Player CreateHumanPlayer(string name)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsHuman = true
        };
    }

    public static Player CreateAiPlayer(string name)
    {
        return new Player
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsHuman = false
        };
    }

    // ==================== Nation ====================

    public static Nation CreateDefaultNation()
    {
        return new Nation();
    }

    public static Nation CreateNation(string name, string flagColorHex = "#FF0000")
    {
        return new Nation
        {
            Id = Guid.NewGuid(),
            Name = name,
            FlagColorHex = flagColorHex
        };
    }

    // ==================== BaseNaturalSolarObject ====================

    public static BaseNaturalSolarObject CreateStar(string name)
    {
        return new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = name,
            BodyType = CelestialBodyType.Star,
            Mass = 1000000f,
            Radius = 50000f,
            PositionX = 0,
            PositionY = 0
        };
    }

    public static BaseNaturalSolarObject CreatePlanet(string name, CelestialBodyType type = CelestialBodyType.Planet)
    {
        return new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = name,
            BodyType = type,
            Mass = 1000f,
            Radius = 100f,
            PositionX = 1000f,
            PositionY = 0,
            OrbitalPeriod = 365f,
            SemiMajorAxis = 1000f,
            Eccentricity = 0.02f,
            SunlightLevel = 100
        };
    }

    public static BaseNaturalSolarObject CreateMoon(string name, Guid parentId)
    {
        return new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = name,
            BodyType = CelestialBodyType.Moon,
            Mass = 100f,
            Radius = 20f,
            ParentBodyId = parentId,
            PositionX = 1050f,
            PositionY = 0,
            OrbitalPeriod = 27f,
            SemiMajorAxis = 50f,
            Eccentricity = 0.05f,
            SunlightLevel = 80
        };
    }

    // ==================== PlanetarySystem ====================

    public static PlanetarySystem CreateDefaultPlanetarySystem()
    {
        return new PlanetarySystem();
    }

    public static PlanetarySystem CreatePlanetarySystem(BaseNaturalSolarObject centralMass)
    {
        return new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = $"{centralMass.Name} System",
            CentralMassId = centralMass.Id,
            CentralMass = centralMass,
            CelestialBodyIds = new List<Guid>(),
            CelestialBodies = new List<BaseNaturalSolarObject>(),
            PlanetarySystemIds = new List<Guid>(),
            PlanetarySystems = new List<PlanetarySystem>()
        };
    }

    // ==================== SolarSystem ====================

    public static SolarSystem CreateDefaultSolarSystem()
    {
        var star = CreateStar("Sol");
        return new SolarSystem
        {
            Id = Guid.NewGuid(),
            Name = "Solar System",
            CentralMassId = star.Id,
            CentralMass = star,
            PlanetarySystemIds = new List<Guid>(),
            PlanetarySystems = new List<PlanetarySystem>()
        };
    }

    // ==================== Resources ====================

    public static GameResource CreateMineralResource(int quantity = 1000)
    {
        return new GameResource
        {
            Name = "Minerals",
            ResourceType = ResourceTypes.Minerals,
            Quantity = quantity,
            MaxQuantity = int.MaxValue
        };
    }

    public static GameResource CreateEnergyResource(int quantity = 500)
    {
        return new GameResource
        {
            Name = "Energy",
            ResourceType = ResourceTypes.Energy,
            Quantity = quantity,
            MaxQuantity = int.MaxValue
        };
    }

    // ==================== Research ====================

    public static ResearchItem CreateDefaultResearchItem()
    {
        return new ResearchItem();
    }

    public static ResearchItem CreateResearchItem(string name, ResearchStates state = ResearchStates.None)
    {
        return new ResearchItem
        {
            Name = name,
            Description = $"Description for {name}",
            ResearchState = state
        };
    }

    // ==================== DTOs ====================

    public static PlanetGameObjectDto CreatePlanetDto()
    {
        return new PlanetGameObjectDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Planet",
            BodyType = CelestialBodyType.Planet,
            PositionX = 100f,
            PositionY = 200f,
            Mass = 1000f,
            Radius = 100f,
            OrbitalPeriod = 365f,
            SemiMajorAxis = 1000f,
            Eccentricity = 0.02f,
            SunlightLevel = 100,
            ResourceDeposits = new List<GameResource>(),
            Orbits = new List<Orbit>(),
            LandingSites = new List<LandingSite>()
        };
    }

    public static StarGameObjectDto CreateStarDto()
    {
        return new StarGameObjectDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Star",
            BodyType = CelestialBodyType.Star,
            PositionX = 0f,
            PositionY = 0f,
            Mass = 1000000f,
            Radius = 50000f,
            OrbitalPeriod = 0f,
            SemiMajorAxis = 0f,
            Eccentricity = 0f,
            SiblingBodyIds = new List<Guid>(),
            ResourceDeposits = new List<GameResource>(),
            Orbits = new List<Orbit>()
        };
    }

    public static PlanetarySystemGameObjectDto CreatePlanetarySystemDto()
    {
        return new PlanetarySystemGameObjectDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Planetary System",
            CentralMassId = Guid.NewGuid(),
            CelestialBodyIds = new List<Guid>(),
            PlanetarySystemIds = new List<Guid>()
        };
    }

    // ==================== GameSettings ====================

    public static GameSettings CreateDefaultGameSettings()
    {
        return new GameSettings
        {
            SessionName = "Test Session",
            PlayerName = "Test Player",
            NumberOfPlanets = 5,
            StarType = StarType.MainSequence,
            AiPlayerCount = 0,
            StartingMineralModifier = 1,
            StartingEnergyModifier = 1,
            PlayerNationId = Guid.NewGuid()
        };
    }

    // ==================== Owned Entities ====================

    public static Orbit CreateOrbit(string name = "Orbit 1")
    {
        return new Orbit
        {
            Name = name,
            MaxStationaryArtificialObjects = 10,
            MaxMobileArtificialObjects = 5,
            CurrentStationaryArtificialObjects = 0,
            CurrentMobileArtificialObjects = 0
        };
    }

    public static LandingSite CreateLandingSite(string name = "Landing Site 1")
    {
        return new LandingSite
        {
            Name = name,
            MaxStationaryArtificialObjects = 5,
            MaxMobileArtificialObjects = 3,
            CurrentStationaryArtificialObjects = 0,
            CurrentMobileArtificialObjects = 0
        };
    }

    public static ResearchState CreateResearchState()
    {
        return new ResearchState
        {
            FinishedResearch = new List<ResearchItem>(),
            CurrentResearch = null,
            ResearchQueue = new List<ResearchItem>(),
            KnownResearch = new List<ResearchItem>()
        };
    }

    public static ResourcesState CreateResourcesState()
    {
        return new ResourcesState
        {
            Resources = new List<GameResource>()
        };
    }

    // ==================== StrategicWorldState ====================

    public static StrategicWorldState CreateDefaultStrategicWorldState()
    {
        return new StrategicWorldState
        {
            Id = Guid.NewGuid(),
            Name = "Test Strategic World"
        };
    }
}
