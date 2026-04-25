using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.UI.MenuControllers;
using AlienInvasionLogistics.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace AlienInvasionLogistics.Tests.Models;

public class GameSessionTests
{
    [Fact]
    public void GameSession_DefaultValues_ShouldBeSet()
    {
        var session = new GameSession();

        session.Id.Should().NotBe(Guid.Empty);
        session.SessionName.Should().Be("New Game");
        session.SaveName.Should().Be("New Save");
        session.PlayerName.Should().Be("New Player");
        session.InGameDay.Should().Be(0);
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        session.LastSavedAt.Should().BeNull();
    }

    [Fact]
    public void GameSession_NewGuid_ShouldGenerateUniqueIds()
    {
        var session1 = new GameSession();
        var session2 = new GameSession();

        session1.Id.Should().NotBe(session2.Id);
    }

    [Fact]
    public void GameSession_PropertyAccess_ShouldWork()
    {
        var session = new GameSession
        {
            SessionName = "Test Session",
            SaveName = "Test Save",
            PlayerName = "Test Player",
            InGameDay = 10,
            NumberOfAiPlayers = 3,
            NumberOfPlanets = 8,
            StarType = StarType.RedDwarf,
            StartingMineralModifier = 2,
            StartingEnergyModifier = 3
        };

        session.SessionName.Should().Be("Test Session");
        session.SaveName.Should().Be("Test Save");
        session.PlayerName.Should().Be("Test Player");
        session.InGameDay.Should().Be(10);
        session.NumberOfAiPlayers.Should().Be(3);
        session.NumberOfPlanets.Should().Be(8);
        session.StarType.Should().Be(StarType.RedDwarf);
        session.StartingMineralModifier.Should().Be(2);
        session.StartingEnergyModifier.Should().Be(3);
    }

    [Fact]
    public void GameSession_Collections_ShouldBeEmptyByDefault()
    {
        var session = new GameSession();

        session.Players.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void GameSession_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var session = TestDataBuilder.CreateGameSession("My Game", "John", 10);

        session.SessionName.Should().Be("My Game");
        session.PlayerName.Should().Be("John");
        session.NumberOfPlanets.Should().Be(10);
        session.SaveName.Should().Contain("John");
        session.SaveName.Should().Contain("My Game");
    }
}

public class PlayerTests
{
    [Fact]
    public void Player_DefaultValues_ShouldBeSet()
    {
        var player = new Player();

        player.Id.Should().NotBe(Guid.Empty);
        player.Name.Should().Be("New Player");
        player.IsHuman.Should().BeFalse();
    }

    [Fact]
    public void Player_IsHuman_DefaultsFalse()
    {
        var player = new Player();

        player.IsHuman.Should().BeFalse();
    }

    [Fact]
    public void Player_PropertyAccess_ShouldWork()
    {
        var sessionId = Guid.NewGuid();
        var nationId = Guid.NewGuid();

        var player = new Player
        {
            Name = "Test Player",
            IsHuman = true,
            GameSessionId = sessionId,
            NationId = nationId
        };

        player.Name.Should().Be("Test Player");
        player.IsHuman.Should().BeTrue();
        player.GameSessionId.Should().Be(sessionId);
        player.NationId.Should().Be(nationId);
    }

    [Fact]
    public void Player_HumanPlayer_ShouldHaveIsHumanTrue()
    {
        var player = TestDataBuilder.CreateHumanPlayer("Human");

        player.IsHuman.Should().BeTrue();
        player.Name.Should().Be("Human");
    }

    [Fact]
    public void Player_AiPlayer_ShouldHaveIsHumanFalse()
    {
        var player = TestDataBuilder.CreateAiPlayer("AI");

        player.IsHuman.Should().BeFalse();
        player.Name.Should().Be("AI");
    }

    [Fact]
    public void Player_ResourceState_DefaultsToNewInstance()
    {
        var player = new Player();

        player.ResourceState.Should().NotBeNull();
    }
}

public class NationTests
{
    [Fact]
    public void Nation_DefaultValues_ShouldBeSet()
    {
        var nation = new Nation();

        nation.Id.Should().NotBe(Guid.Empty);
        nation.Name.Should().Be("Unnamed Nation");
        nation.FlagColorHex.Should().Be("#FFFFFF");
    }

    [Fact]
    public void Nation_FlagColorHex_DefaultsToWhite()
    {
        var nation = new Nation();

        nation.FlagColorHex.Should().Be("#FFFFFF");
    }

    [Fact]
    public void Nation_Collections_ShouldBeEmptyByDefault()
    {
        var nation = new Nation();

        nation.OccupiedCelestialBodies.Should().NotBeNull().And.BeEmpty();
        nation.OwnedStaticObjects.Should().NotBeNull().And.BeEmpty();
        nation.OwnedMobileObjects.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Nation_PropertyAccess_ShouldWork()
    {
        var nation = new Nation
        {
            Name = "Test Nation",
            FlagColorHex = "#FF0000"
        };

        nation.Name.Should().Be("Test Nation");
        nation.FlagColorHex.Should().Be("#FF0000");
    }

    [Fact]
    public void Nation_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var nation = TestDataBuilder.CreateNation("My Nation", "#00FF00");

        nation.Name.Should().Be("My Nation");
        nation.FlagColorHex.Should().Be("#00FF00");
    }
}

public class CelestialBodyTests
{
    [Fact]
    public void CelestialBody_DefaultBodyType_IsPlanet()
    {
        var body = new BaseNaturalSolarObject();

        body.BodyType.Should().Be(CelestialBodyType.Planet);
    }

    [Fact]
    public void CelestialBody_ResourceDeposits_EmptyByDefault()
    {
        var body = new BaseNaturalSolarObject();

        body.ResourceDeposits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CelestialBody_Orbits_EmptyByDefault()
    {
        var body = new BaseNaturalSolarObject();

        body.Orbits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void CelestialBody_LandingSites_NullByDefault()
    {
        var body = new BaseNaturalSolarObject();

        body.LandingSites.Should().BeNull();
    }

    [Fact]
    public void CelestialBody_DefaultValues_ShouldBeSet()
    {
        var body = new BaseNaturalSolarObject();

        body.Id.Should().NotBe(Guid.Empty);
        body.Name.Should().Be("Unnamed Celestial Body");
        body.PositionX.Should().Be(0);
        body.PositionY.Should().Be(0);
        body.Mass.Should().Be(0);
        body.Radius.Should().Be(0);
        body.ParentBodyId.Should().BeNull();
    }

    [Fact]
    public void CelestialBody_Star_ShouldHaveCorrectType()
    {
        var star = TestDataBuilder.CreateStar("Sol");

        star.BodyType.Should().Be(CelestialBodyType.Star);
        star.Name.Should().Be("Sol");
    }

    [Fact]
    public void CelestialBody_Planet_ShouldHaveCorrectType()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");

        planet.BodyType.Should().Be(CelestialBodyType.Planet);
        planet.Name.Should().Be("Earth");
    }

    [Fact]
    public void CelestialBody_Moon_ShouldHaveParentBodyId()
    {
        var planetId = Guid.NewGuid();
        var moon = TestDataBuilder.CreateMoon("Luna", planetId);

        moon.BodyType.Should().Be(CelestialBodyType.Moon);
        moon.ParentBodyId.Should().Be(planetId);
        moon.Name.Should().Be("Luna");
    }

    [Theory]
    [InlineData(CelestialBodyType.Star)]
    [InlineData(CelestialBodyType.GasGiant)]
    [InlineData(CelestialBodyType.IceGiant)]
    [InlineData(CelestialBodyType.LargePlanet)]
    [InlineData(CelestialBodyType.Planet)]
    [InlineData(CelestialBodyType.DwarfPlanet)]
    [InlineData(CelestialBodyType.MinorPlanet)]
    [InlineData(CelestialBodyType.Moon)]
    [InlineData(CelestialBodyType.Asteroid)]
    [InlineData(CelestialBodyType.Comet)]
    public void CelestialBody_AllBodyTypes_ShouldBeSettable(CelestialBodyType bodyType)
    {
        var body = new BaseNaturalSolarObject { BodyType = bodyType };

        body.BodyType.Should().Be(bodyType);
    }
}

public class GameResourceTests
{
    [Fact]
    public void GameResource_DefaultValues_ShouldBeSet()
    {
        var resource = new GameResource();

        resource.ResourceType.Should().Be(ResourceTypes.Minerals);
        resource.Name.Should().Be("Minerals");
        resource.Quantity.Should().Be(0);
        resource.MaxQuantity.Should().Be(int.MaxValue);
    }

    [Fact]
    public void GameResource_ResourceType_DefaultsMinerals()
    {
        var resource = new GameResource();

        resource.ResourceType.Should().Be(ResourceTypes.Minerals);
    }

    [Fact]
    public void GameResource_MaxQuantity_DefaultsIntMaxValue()
    {
        var resource = new GameResource();

        resource.MaxQuantity.Should().Be(int.MaxValue);
    }

    [Fact]
    public void GameResource_Mineral_CreatedWithBuilder()
    {
        var resource = TestDataBuilder.CreateMineralResource(500);

        resource.ResourceType.Should().Be(ResourceTypes.Minerals);
        resource.Quantity.Should().Be(500);
    }

    [Fact]
    public void GameResource_Energy_CreatedWithBuilder()
    {
        var resource = TestDataBuilder.CreateEnergyResource(250);

        resource.ResourceType.Should().Be(ResourceTypes.Energy);
        resource.Quantity.Should().Be(250);
    }
}

public class ResearchItemTests
{
    [Fact]
    public void ResearchItem_DefaultValues_ShouldBeSet()
    {
        var item = new ResearchItem();

        item.Name.Should().Be("Research Name");
        item.Description.Should().Be("Research Description");
        item.ResearchState.Should().Be(ResearchStates.None);
    }

    [Fact]
    public void ResearchItem_ResearchState_DefaultsToNone()
    {
        var item = new ResearchItem();

        item.ResearchState.Should().Be(ResearchStates.None);
    }

    [Fact]
    public void ResearchItem_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var item = TestDataBuilder.CreateResearchItem("Laser Tech", ResearchStates.Completed);

        item.Name.Should().Be("Laser Tech");
        item.Description.Should().Contain("Laser Tech");
        item.ResearchState.Should().Be(ResearchStates.Completed);
    }

    [Theory]
    [InlineData(ResearchStates.None)]
    [InlineData(ResearchStates.Queued)]
    [InlineData(ResearchStates.InProgress)]
    [InlineData(ResearchStates.Completed)]
    public void ResearchItem_AllStates_ShouldBeSettable(ResearchStates state)
    {
        var item = new ResearchItem { ResearchState = state };

        item.ResearchState.Should().Be(state);
    }
}

public class OrbitTests
{
    [Fact]
    public void Orbit_DefaultName_ShouldBeOrbit1()
    {
        var orbit = new Orbit();

        orbit.Name.Should().Be("Orbit 1");
    }

    [Fact]
    public void Orbit_ArtificialObjectCollections_EmptyByDefault()
    {
        var orbit = new Orbit();

        orbit.StaticArtificialObjects.Should().NotBeNull().And.BeEmpty();
        orbit.MobileArtificialObjects.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Orbit_PropertyAccess_ShouldWork()
    {
        var orbit = new Orbit
        {
            Name = "High Orbit",
            MaxStationaryArtificialObjects = 20,
            MaxMobileArtificialObjects = 10,
            CurrentStationaryArtificialObjects = 5,
            CurrentMobileArtificialObjects = 3
        };

        orbit.Name.Should().Be("High Orbit");
        orbit.MaxStationaryArtificialObjects.Should().Be(20);
        orbit.MaxMobileArtificialObjects.Should().Be(10);
        orbit.CurrentStationaryArtificialObjects.Should().Be(5);
        orbit.CurrentMobileArtificialObjects.Should().Be(3);
    }
}

public class LandingSiteTests
{
    [Fact]
    public void LandingSite_DefaultName_ShouldBeLandingSite1()
    {
        var site = new LandingSite();

        site.Name.Should().Be("Landing Site 1");
    }

    [Fact]
    public void LandingSite_PropertyAccess_ShouldWork()
    {
        var site = new LandingSite
        {
            Name = "Base Alpha",
            MaxStationaryArtificialObjects = 15,
            MaxMobileArtificialObjects = 8,
            CurrentStationaryArtificialObjects = 2,
            CurrentMobileArtificialObjects = 1
        };

        site.Name.Should().Be("Base Alpha");
        site.MaxStationaryArtificialObjects.Should().Be(15);
        site.MaxMobileArtificialObjects.Should().Be(8);
        site.CurrentStationaryArtificialObjects.Should().Be(2);
        site.CurrentMobileArtificialObjects.Should().Be(1);
    }

    [Fact]
    public void LandingSite_ArtificialObjectCollections_EmptyByDefault()
    {
        var site = new LandingSite();

        site.StaticArtificialObjects.Should().NotBeNull().And.BeEmpty();
        site.MobileArtificialObjects.Should().NotBeNull().And.BeEmpty();
    }
}

public class PlanetarySystemTests
{
    [Fact]
    public void PlanetarySystem_DefaultValues_ShouldBeSet()
    {
        var system = new PlanetarySystem();

        system.Id.Should().NotBe(Guid.Empty);
        system.Name.Should().Be("Unnamed Celestial Body");
    }

    [Fact]
    public void PlanetarySystem_NullableCollections_DefaultNull()
    {
        var system = new PlanetarySystem();

        system.CelestialBodies.Should().BeNull();
    }

    [Fact]
    public void PlanetarySystem_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        var system = TestDataBuilder.CreatePlanetarySystem(planet);

        system.CentralMassId.Should().Be(planet.Id);
        system.CentralMass.Should().Be(planet);
        system.Name.Should().Contain("Earth");
    }
}

public class SolarSystemTests
{
    [Fact]
    public void SolarSystem_DefaultValues_ShouldBeSet()
    {
        var system = new SolarSystem();

        system.Id.Should().NotBe(Guid.Empty);
        system.Name.Should().Be("Unnamed Celestial Body");
    }

    [Fact]
    public void SolarSystem_PlanetarySystems_EmptyByDefault()
    {
        var system = new SolarSystem();

        system.PlanetarySystems.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void SolarSystem_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var system = TestDataBuilder.CreateDefaultSolarSystem();

        system.Name.Should().Be("Solar System");
        system.CentralMass.Should().NotBeNull();
        system.CentralMass.BodyType.Should().Be(CelestialBodyType.Star);
        system.PlanetarySystems.Should().NotBeNull().And.BeEmpty();
    }
}

public class StrategicWorldStateTests
{
    [Fact]
    public void StrategicWorldState_DefaultValues_ShouldBeSet()
    {
        var state = new StrategicWorldState();

        state.Id.Should().NotBe(Guid.Empty);
        state.Name.Should().Be("Unnamed Strategic World");
    }

    [Fact]
    public void StrategicWorldState_Collections_EmptyByDefault()
    {
        var state = new StrategicWorldState();

        state.Nations.Should().NotBeNull().And.BeEmpty();
        state.Players.Should().NotBeNull().And.BeEmpty();
    }
}

public class ResearchStateTests
{
    [Fact]
    public void ResearchState_CreatedWithBuilder_ShouldHaveEmptyCollections()
    {
        var state = TestDataBuilder.CreateResearchState();

        state.FinishedResearch.Should().NotBeNull().And.BeEmpty();
        state.ResearchQueue.Should().NotBeNull().And.BeEmpty();
        state.KnownResearch.Should().NotBeNull().And.BeEmpty();
        state.CurrentResearch.Should().BeNull();
    }
}

public class ResourcesStateTests
{
    [Fact]
    public void ResourcesState_CreatedWithBuilder_ShouldHaveEmptyResources()
    {
        var state = TestDataBuilder.CreateResourcesState();

        state.Resources.Should().NotBeNull().And.BeEmpty();
    }
}
