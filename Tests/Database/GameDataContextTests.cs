using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database.Contexts;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AlienInvasionLogistics.Tests.Database;

public class GameDataContextTests : IDisposable
{
    private readonly GameDataContext _context;

    public GameDataContextTests()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    // ==================== DbSet Tests ====================

    [Fact]
    public void DbSets_GameSessions_ShouldNotBeNull()
    {
        _context.GameSessions.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_Players_ShouldNotBeNull()
    {
        _context.Players.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_Nations_ShouldNotBeNull()
    {
        _context.Nations.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_StrategicWorldStates_ShouldNotBeNull()
    {
        _context.StrategicWorldStates.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_SolarSystems_ShouldNotBeNull()
    {
        _context.SolarSystems.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_PlanetarySystems_ShouldNotBeNull()
    {
        _context.PlanetarySystems.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_CelestialBodies_ShouldNotBeNull()
    {
        _context.CelestialBodies.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_StaticArtificialObjects_ShouldNotBeNull()
    {
        _context.StaticArtificialObjects.Should().NotBeNull();
    }

    [Fact]
    public void DbSets_MobileArtificialObjects_ShouldNotBeNull()
    {
        _context.MobileArtificialObjects.Should().NotBeNull();
    }

    // ==================== CRUD Tests ====================

    [Fact]
    public async Task GameSession_CanBeAddedAndRetrieved()
    {
        var session = TestDataBuilder.CreateGameSession("Test Game", "Player1");

        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var retrieved = await _context.GameSessions.FindAsync(session.Id);
        retrieved.Should().NotBeNull();
        retrieved.SessionName.Should().Be("Test Game");
    }

    [Fact]
    public async Task Player_CanBeAddedAndRetrieved()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Players.FindAsync(player.Id);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("Test Player");
    }

    [Fact]
    public async Task Nation_CanBeAddedAndRetrieved()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);

        var worldState = TestDataBuilder.CreateDefaultStrategicWorldState();
        _context.StrategicWorldStates.Add(worldState);
        await _context.SaveChangesAsync();

        var nation = TestDataBuilder.CreateNation("Test Nation", "#FF0000");
        nation.PlayerId = player.Id;
        nation.StrategicWorldStateId = worldState.Id;
        _context.Nations.Add(nation);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Nations.FindAsync(nation.Id);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("Test Nation");
    }

    [Fact]
    public async Task CelestialBody_CanBeAddedAndRetrieved()
    {
        var body = TestDataBuilder.CreatePlanet("Earth");

        _context.CelestialBodies.Add(body);
        await _context.SaveChangesAsync();

        var retrieved = await _context.CelestialBodies.FindAsync(body.Id);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("Earth");
    }

    [Fact]
    public async Task SolarSystem_CanBeAddedAndRetrieved()
    {
        var star = TestDataBuilder.CreateStar("Sol");
        _context.CelestialBodies.Add(star);
        await _context.SaveChangesAsync();

        var system = new SolarSystem
        {
            Id = Guid.NewGuid(),
            Name = "Solar System",
            CentralMassId = star.Id,
        };
        _context.SolarSystems.Add(system);
        await _context.SaveChangesAsync();

        var retrieved = await _context.SolarSystems.FindAsync(system.Id);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("Solar System");
    }

    [Fact]
    public async Task PlanetarySystem_CanBeAddedAndRetrieved()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var system = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = "Earth System",
            CentralMassId = planet.Id,
        };
        _context.PlanetarySystems.Add(system);
        await _context.SaveChangesAsync();

        var retrieved = await _context.PlanetarySystems.FindAsync(system.Id);
        retrieved.Should().NotBeNull();
        retrieved.Name.Should().Be("Earth System");
    }

    // ==================== Relationship Tests ====================

    [Fact]
    public async Task GameSession_HasPlayers_Relationship()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var player = TestDataBuilder.CreateHumanPlayer("Player One");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var retrieved = await _context.GameSessions
            .Include(gs => gs.Players)
            .FirstOrDefaultAsync(gs => gs.Id == session.Id);

        retrieved.Players.Should().HaveCount(1);
        retrieved.Players.First().Name.Should().Be("Player One");
    }

    [Fact]
    public async Task Player_HasNation_Relationship()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);

        var worldState = TestDataBuilder.CreateDefaultStrategicWorldState();
        _context.StrategicWorldStates.Add(worldState);

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var nation = TestDataBuilder.CreateNation("Human Empire");
        nation.PlayerId = player.Id;
        nation.StrategicWorldStateId = worldState.Id;
        _context.Nations.Add(nation);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Players
            .Include(p => p.Nation)
            .FirstOrDefaultAsync(p => p.Id == player.Id);

        retrieved.Nation.Should().NotBeNull();
        retrieved.Nation.Name.Should().Be("Human Empire");
    }

    [Fact]
    public async Task SolarSystem_HasPlanetarySystems_Relationship()
    {
        var star = TestDataBuilder.CreateStar("Sol");
        _context.CelestialBodies.Add(star);

        var planet = TestDataBuilder.CreatePlanet("Earth");
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var planetarySystem = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = "Earth System",
            CentralMassId = planet.Id,
        };

        var solarSystem = new SolarSystem
        {
            Id = Guid.NewGuid(),
            Name = "Solar System",
            CentralMassId = star.Id,
            PlanetarySystems = new List<PlanetarySystem> { planetarySystem }
        };
        _context.SolarSystems.Add(solarSystem);
        await _context.SaveChangesAsync();

        var retrieved = await _context.SolarSystems
            .Include(ss => ss.PlanetarySystems)
            .FirstOrDefaultAsync(ss => ss.Id == solarSystem.Id);

        retrieved.PlanetarySystems.Should().HaveCount(1);
    }

    [Fact]
    public async Task CelestialBody_ParentChild_Relationship()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var moon = TestDataBuilder.CreateMoon("Luna", planet.Id);
        _context.CelestialBodies.Add(moon);
        await _context.SaveChangesAsync();

        var retrievedPlanet = await _context.CelestialBodies
            .Include(cb => cb.ChildBodies)
            .FirstOrDefaultAsync(cb => cb.Id == planet.Id);

        var retrievedMoon = await _context.CelestialBodies
            .Include(cb => cb.ParentBody)
            .FirstOrDefaultAsync(cb => cb.Id == moon.Id);

        retrievedPlanet.ChildBodies.Should().HaveCount(1);
        retrievedMoon.ParentBody.Should().NotBeNull();
        retrievedMoon.ParentBody.Name.Should().Be("Earth");
    }

    // ==================== Owned Entity Tests ====================

    [Fact]
    public async Task Player_OwnsResearchState()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        player.ResearchState = TestDataBuilder.CreateResearchState();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Players.FindAsync(player.Id);
        // Owned entities are automatically included
        retrieved.Should().NotBeNull();
    }

    [Fact]
    public async Task Player_OwnsResourceState()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        player.ResourceState = TestDataBuilder.CreateResourcesState();
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Players.FindAsync(player.Id);
        retrieved.Should().NotBeNull();
    }

    [Fact]
    public async Task CelestialBody_OwnsResourceDeposits()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        planet.ResourceDeposits.Add(TestDataBuilder.CreateMineralResource(500));
        planet.ResourceDeposits.Add(TestDataBuilder.CreateEnergyResource(200));
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var retrieved = await _context.CelestialBodies.FindAsync(planet.Id);
        retrieved.ResourceDeposits.Should().HaveCount(2);
    }

    [Fact]
    public async Task CelestialBody_OwnsOrbits()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        planet.Orbits.Add(TestDataBuilder.CreateOrbit("Low Orbit"));
        planet.Orbits.Add(TestDataBuilder.CreateOrbit("High Orbit"));
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var retrieved = await _context.CelestialBodies.FindAsync(planet.Id);
        retrieved.Orbits.Should().HaveCount(2);
    }

    [Fact]
    public async Task CelestialBody_OwnsLandingSites()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        planet.LandingSites = new System.Collections.Generic.List<LandingSite>
        {
            TestDataBuilder.CreateLandingSite("Alpha Base"),
            TestDataBuilder.CreateLandingSite("Beta Base")
        };
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var retrieved = await _context.CelestialBodies.FindAsync(planet.Id);
        retrieved.LandingSites.Should().HaveCount(2);
    }

    // ==================== Cascade Delete Tests ====================

    [Fact]
    public async Task GameSession_DeletingSession_DeletesPlayers()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        _context.GameSessions.Remove(session);
        await _context.SaveChangesAsync();

        var playerCount = await _context.Players.CountAsync();
        playerCount.Should().Be(0);
    }

    [Fact]
    public async Task Player_DeletingPlayer_DeletesNation()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        _context.GameSessions.Add(session);

        var worldState = TestDataBuilder.CreateDefaultStrategicWorldState();
        _context.StrategicWorldStates.Add(worldState);

        var player = TestDataBuilder.CreateHumanPlayer("Test Player");
        player.GameSessionId = session.Id;
        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        var nation = TestDataBuilder.CreateNation("Test Nation");
        nation.PlayerId = player.Id;
        nation.StrategicWorldStateId = worldState.Id;
        _context.Nations.Add(nation);
        await _context.SaveChangesAsync();

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        var nationCount = await _context.Nations.CountAsync();
        nationCount.Should().Be(0);
    }

    [Fact]
    public async Task SolarSystem_DeletingSystem_DeletesPlanetarySystems()
    {
        var star = TestDataBuilder.CreateStar("Sol");
        _context.CelestialBodies.Add(star);

        var planet = TestDataBuilder.CreatePlanet("Earth");
        _context.CelestialBodies.Add(planet);
        await _context.SaveChangesAsync();

        var planetarySystem = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = "Earth System",
            CentralMassId = planet.Id,
        };

        var solarSystem = new SolarSystem
        {
            Id = Guid.NewGuid(),
            Name = "Solar System",
            CentralMassId = star.Id,
            PlanetarySystems = new List<PlanetarySystem> { planetarySystem }
        };
        _context.SolarSystems.Add(solarSystem);
        await _context.SaveChangesAsync();

        _context.SolarSystems.Remove(solarSystem);
        await _context.SaveChangesAsync();

        var psCount = await _context.PlanetarySystems.CountAsync();
        psCount.Should().Be(0);
    }

    // ==================== Query Tests ====================

    [Fact]
    public async Task GameSession_CanQueryBySaveName()
    {
        var session1 = TestDataBuilder.CreateGameSession("Game1", "Player1");
        var session2 = TestDataBuilder.CreateGameSession("Game2", "Player2");
        _context.GameSessions.AddRange(session1, session2);
        await _context.SaveChangesAsync();

        var result = await _context.GameSessions
            .Where(gs => gs.SaveName.Contains("Player1"))
            .ToListAsync();

        result.Should().HaveCount(1);
        result.First().PlayerName.Should().Be("Player1");
    }

    [Fact]
    public async Task CelestialBody_CanQueryByBodyType()
    {
        var star = TestDataBuilder.CreateStar("Sol");
        var planet1 = TestDataBuilder.CreatePlanet("Earth");
        var planet2 = TestDataBuilder.CreatePlanet("Mars");
        var moon = TestDataBuilder.CreateMoon("Luna", planet1.Id);

        _context.CelestialBodies.AddRange(star, planet1, planet2, moon);
        await _context.SaveChangesAsync();

        var planets = await _context.CelestialBodies
            .Where(cb => cb.BodyType == CelestialBodyType.Planet)
            .ToListAsync();

        planets.Should().HaveCount(2);
    }

    [Fact]
    public async Task GameSession_MultiplePlayersPerSession()
    {
        var session = TestDataBuilder.CreateGameSession("Multiplayer", "Host");
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        var player1 = TestDataBuilder.CreateHumanPlayer("Player 1");
        player1.GameSessionId = session.Id;
        var player2 = TestDataBuilder.CreateAiPlayer("AI 1");
        player2.GameSessionId = session.Id;
        var player3 = TestDataBuilder.CreateAiPlayer("AI 2");
        player3.GameSessionId = session.Id;

        _context.Players.AddRange(player1, player2, player3);
        await _context.SaveChangesAsync();

        var retrieved = await _context.GameSessions
            .Include(gs => gs.Players)
            .FirstOrDefaultAsync(gs => gs.Id == session.Id);

        retrieved.Players.Should().HaveCount(3);
        retrieved.Players.Count(p => p.IsHuman).Should().Be(1);
        retrieved.Players.Count(p => !p.IsHuman).Should().Be(2);
    }
}
