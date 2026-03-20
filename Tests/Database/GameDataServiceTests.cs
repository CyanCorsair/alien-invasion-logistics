using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database;
using AlienInvasionLogistics.Source.Database.Contexts;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.UI.MenuControllers;
using AlienInvasionLogistics.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace AlienInvasionLogistics.Tests.Database;

/// <summary>
/// Testable version of GameDataService that doesn't extend Godot.Node
/// Mirrors the functionality of GameDataService for testing purposes
/// </summary>
public class TestableGameDataService : IGameDataService
{
    private IGameDataContextFactory _contextFactory;
    private IEventBus _eventBus;

    public void Initialize(IGameDataContextFactory contextFactory, IEventBus eventBus)
    {
        _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task CreateNewGameAsync(GameSettings settings)
    {
        await using var context = _contextFactory.CreateDbContext();

        // Note: InMemory database doesn't support transactions, so we skip them in tests
        var gameSession = new GameSession()
        {
            SessionName = settings.SessionName,
            SaveName = $"{settings.PlayerName} - {settings.SessionName} - Day 0",
            InGameDay = 0,
            CreatedAt = DateTime.UtcNow,
            NumberOfAiPlayers = settings.AiPlayerCount,
            NumberOfPlanets = settings.NumberOfPlanets,
            StarType = settings.StarType,
            StartingMineralModifier = settings.StartingMineralModifier,
            StartingEnergyModifier = settings.StartingEnergyModifier,
        };

        context.GameSessions.Add(gameSession);
        await context.SaveChangesAsync();

        _eventBus.Publish(new GameCreatedEvent(gameSession.Id, settings.PlayerName));
    }

    public async Task SaveGameAsync(GameSession newSession)
    {
        await using var context = _contextFactory.CreateDbContext();
        await context.GameSessions.AddAsync(newSession);
        await context.SaveChangesAsync();
        _eventBus.Publish(new GameSavedEvent(newSession.Id, newSession.SaveName));
    }

    public async Task ClearDatabaseAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public async Task<List<GameSession>> GetAllSavesAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.GameSessions.ToListAsync();
    }

    public async Task<GameSession?> LoadGameAsync(string saveId)
    {
        if (string.IsNullOrWhiteSpace(saveId) || !Guid.TryParse(saveId, out var guid))
        {
            return null;
        }

        return await LoadGameAsync(guid);
    }

    public async Task<GameSession?> LoadGameAsync(Guid saveId)
    {
        await using var context = _contextFactory.CreateDbContext();

        var save = await context.GameSessions
            .Include(gs => gs.Players)
            .FirstOrDefaultAsync(gs => gs.Id == saveId);

        if (save != null)
        {
            _eventBus.Publish(new GameLoadedEvent(save.Id, save.SessionName));
        }

        return save;
    }
}

/// <summary>
/// Test context factory for creating in-memory databases
/// </summary>
public class TestGameDataContextFactory : IGameDataContextFactory
{
    private readonly DbContextOptions<GameDataContext> _options;

    public TestGameDataContextFactory(string? databaseName = null)
    {
        _options = new DbContextOptionsBuilder<GameDataContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    public GameDataContext CreateDbContext()
    {
        return new GameDataContext(_options);
    }
}

public class GameDataServiceTests : IDisposable
{
    private readonly TestableGameDataService _service;
    private readonly TestGameDataContextFactory _contextFactory;
    private readonly Mock<IEventBus> _mockEventBus;

    public GameDataServiceTests()
    {
        _contextFactory = new TestGameDataContextFactory();
        _mockEventBus = new Mock<IEventBus>();
        _service = new TestableGameDataService();
        _service.Initialize(_contextFactory, _mockEventBus.Object);
    }

    public void Dispose()
    {
    }

    // ==================== Initialize Tests ====================

    [Fact]
    public void Initialize_WithValidDependencies_Succeeds()
    {
        var service = new TestableGameDataService();
        var factory = new TestGameDataContextFactory();
        var mockEventBus = new Mock<IEventBus>();

        var act = () => service.Initialize(factory, mockEventBus.Object);

        act.Should().NotThrow();
    }

    [Fact]
    public void Initialize_NullFactory_ThrowsArgumentNullException()
    {
        var service = new TestableGameDataService();
        var mockEventBus = new Mock<IEventBus>();

        Action act = () => service.Initialize(null, mockEventBus.Object);

        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("contextFactory");
    }

    [Fact]
    public void Initialize_NullEventBus_ThrowsArgumentNullException()
    {
        var service = new TestableGameDataService();
        var factory = new TestGameDataContextFactory();

        Action act = () => service.Initialize(factory, null);

        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("eventBus");
    }

    // ==================== CreateNewGameAsync Tests ====================

    [Fact]
    public async Task CreateNewGameAsync_ValidSettings_CreatesSession()
    {
        var settings = TestDataBuilder.CreateDefaultGameSettings();

        await _service.CreateNewGameAsync(settings);

        var saves = await _service.GetAllSavesAsync();
        saves.Should().HaveCount(1);
        saves.First().SessionName.Should().Be(settings.SessionName);
    }

    [Fact]
    public async Task CreateNewGameAsync_ValidSettings_PublishesGameCreatedEvent()
    {
        var settings = TestDataBuilder.CreateDefaultGameSettings();

        await _service.CreateNewGameAsync(settings);

        _mockEventBus.Verify(
            eb => eb.Publish(It.Is<GameCreatedEvent>(e => e.PlayerName == settings.PlayerName)),
            Times.Once);
    }

    [Fact]
    public async Task CreateNewGameAsync_ValidSettings_SavesToDatabaseWithCorrectProperties()
    {
        var settings = new GameSettings
        {
            SessionName = "Epic Game",
            PlayerName = "Hero",
            NumberOfPlanets = 8,
            StarType = StarType.BlueGiant,
            AiPlayerCount = 3,
            StartingMineralModifier = 2,
            StartingEnergyModifier = 3
        };

        await _service.CreateNewGameAsync(settings);

        var saves = await _service.GetAllSavesAsync();
        var session = saves.First();
        session.SessionName.Should().Be("Epic Game");
        session.NumberOfPlanets.Should().Be(8);
        session.StarType.Should().Be(StarType.BlueGiant);
        session.NumberOfAiPlayers.Should().Be(3);
        session.StartingMineralModifier.Should().Be(2);
        session.StartingEnergyModifier.Should().Be(3);
    }

    [Fact]
    public async Task CreateNewGameAsync_SetsUtcTimestamp()
    {
        var settings = TestDataBuilder.CreateDefaultGameSettings();
        var beforeCreate = DateTime.UtcNow;

        await _service.CreateNewGameAsync(settings);

        var afterCreate = DateTime.UtcNow;
        var saves = await _service.GetAllSavesAsync();
        var session = saves.First();
        session.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        session.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    [Fact]
    public async Task CreateNewGameAsync_SetsSaveNameWithPlayerAndSessionName()
    {
        var settings = new GameSettings
        {
            SessionName = "Test Session",
            PlayerName = "Test Player"
        };

        await _service.CreateNewGameAsync(settings);

        var saves = await _service.GetAllSavesAsync();
        var session = saves.First();
        session.SaveName.Should().Contain("Test Player");
        session.SaveName.Should().Contain("Test Session");
        session.SaveName.Should().Contain("Day 0");
    }

    [Fact]
    public async Task CreateNewGameAsync_SetsInGameDayToZero()
    {
        var settings = TestDataBuilder.CreateDefaultGameSettings();

        await _service.CreateNewGameAsync(settings);

        var saves = await _service.GetAllSavesAsync();
        saves.First().InGameDay.Should().Be(0);
    }

    // ==================== SaveGameAsync Tests ====================

    [Fact]
    public async Task SaveGameAsync_ValidSession_SavesSession()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");

        await _service.SaveGameAsync(session);

        var saves = await _service.GetAllSavesAsync();
        saves.Should().HaveCount(1);
        saves.First().Id.Should().Be(session.Id);
    }

    [Fact]
    public async Task SaveGameAsync_ValidSession_PublishesGameSavedEvent()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");

        await _service.SaveGameAsync(session);

        _mockEventBus.Verify(
            eb => eb.Publish(It.Is<GameSavedEvent>(e =>
                e.SaveId == session.Id &&
                e.SaveName == session.SaveName)),
            Times.Once);
    }

    [Fact]
    public async Task SaveGameAsync_MultipleSessions_SavesAll()
    {
        var session1 = TestDataBuilder.CreateGameSession("Game1", "Player1");
        var session2 = TestDataBuilder.CreateGameSession("Game2", "Player2");

        await _service.SaveGameAsync(session1);
        await _service.SaveGameAsync(session2);

        var saves = await _service.GetAllSavesAsync();
        saves.Should().HaveCount(2);
    }

    // ==================== GetAllSavesAsync Tests ====================

    [Fact]
    public async Task GetAllSavesAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var saves = await _service.GetAllSavesAsync();

        saves.Should().NotBeNull();
        saves.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllSavesAsync_WithSaves_ReturnsAllSaves()
    {
        await _service.SaveGameAsync(TestDataBuilder.CreateGameSession("Game1", "Player1"));
        await _service.SaveGameAsync(TestDataBuilder.CreateGameSession("Game2", "Player2"));
        await _service.SaveGameAsync(TestDataBuilder.CreateGameSession("Game3", "Player3"));

        var saves = await _service.GetAllSavesAsync();

        saves.Should().HaveCount(3);
    }

    // ==================== LoadGameAsync Tests ====================

    [Fact]
    public async Task LoadGameAsync_ValidGuid_ReturnsSession()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        await _service.SaveGameAsync(session);

        var loaded = await _service.LoadGameAsync(session.Id.ToString());

        loaded.Should().NotBeNull();
        loaded.Id.Should().Be(session.Id);
        loaded.SessionName.Should().Be("Test");
    }

    [Fact]
    public async Task LoadGameAsync_InvalidGuid_ReturnsNull()
    {
        var loaded = await _service.LoadGameAsync("not-a-guid");

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_NonExistentGuid_ReturnsNull()
    {
        var loaded = await _service.LoadGameAsync(Guid.NewGuid().ToString());

        loaded.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_ValidGuid_PublishesGameLoadedEvent()
    {
        var session = TestDataBuilder.CreateGameSession("Test", "Player");
        await _service.SaveGameAsync(session);
        _mockEventBus.Reset(); // Reset to clear SaveGameAsync event

        await _service.LoadGameAsync(session.Id.ToString());

        _mockEventBus.Verify(
            eb => eb.Publish(It.Is<GameLoadedEvent>(e =>
                e.GameDataId == session.Id &&
                e.SaveName == session.SessionName)),
            Times.Once);
    }

    [Fact]
    public async Task LoadGameAsync_NonExistent_DoesNotPublishEvent()
    {
        await _service.LoadGameAsync(Guid.NewGuid().ToString());

        _mockEventBus.Verify(
            eb => eb.Publish(It.IsAny<GameLoadedEvent>()),
            Times.Never);
    }

    [Fact]
    public async Task LoadGameAsync_InvalidGuid_DoesNotPublishEvent()
    {
        await _service.LoadGameAsync("invalid");

        _mockEventBus.Verify(
            eb => eb.Publish(It.IsAny<GameLoadedEvent>()),
            Times.Never);
    }

    // ==================== ClearDatabaseAsync Tests ====================

    [Fact]
    public async Task ClearDatabaseAsync_RemovesAllData()
    {
        await _service.SaveGameAsync(TestDataBuilder.CreateGameSession("Game1", "Player1"));
        await _service.SaveGameAsync(TestDataBuilder.CreateGameSession("Game2", "Player2"));
        var savesBefore = await _service.GetAllSavesAsync();
        savesBefore.Should().HaveCount(2);

        await _service.ClearDatabaseAsync();

        var savesAfter = await _service.GetAllSavesAsync();
        savesAfter.Should().BeEmpty();
    }

    [Fact]
    public async Task ClearDatabaseAsync_RecreatesDatabase()
    {
        await _service.ClearDatabaseAsync();

        // After clearing, should be able to add new data
        var session = TestDataBuilder.CreateGameSession("New Game", "New Player");
        await _service.SaveGameAsync(session);

        var saves = await _service.GetAllSavesAsync();
        saves.Should().HaveCount(1);
    }

    [Fact]
    public async Task ClearDatabaseAsync_EmptyDatabase_DoesNotThrow()
    {
        Func<Task> act = async () => await _service.ClearDatabaseAsync();

        await act.Should().NotThrowAsync();
    }

    // ==================== Integration Tests ====================

    [Fact]
    public async Task CreateAndLoadGame_RoundTrip_PreservesData()
    {
        var settings = new GameSettings
        {
            SessionName = "Round Trip Test",
            PlayerName = "Test Player",
            NumberOfPlanets = 7,
            StarType = StarType.RedDwarf,
            AiPlayerCount = 2,
            StartingMineralModifier = 5,
            StartingEnergyModifier = 3
        };

        await _service.CreateNewGameAsync(settings);
        var saves = await _service.GetAllSavesAsync();
        var sessionId = saves.First().Id;

        var loaded = await _service.LoadGameAsync(sessionId.ToString());

        loaded.Should().NotBeNull();
        loaded.SessionName.Should().Be("Round Trip Test");
        loaded.NumberOfPlanets.Should().Be(7);
        loaded.StarType.Should().Be(StarType.RedDwarf);
        loaded.NumberOfAiPlayers.Should().Be(2);
        loaded.StartingMineralModifier.Should().Be(5);
        loaded.StartingEnergyModifier.Should().Be(3);
    }

    [Fact]
    public async Task MultipleGames_CanCreateAndLoadIndependently()
    {
        var settings1 = new GameSettings { SessionName = "Game 1", PlayerName = "Player 1" };
        var settings2 = new GameSettings { SessionName = "Game 2", PlayerName = "Player 2" };

        await _service.CreateNewGameAsync(settings1);
        await _service.CreateNewGameAsync(settings2);

        var saves = await _service.GetAllSavesAsync();
        saves.Should().HaveCount(2);

        var game1 = saves.First(s => s.SessionName == "Game 1");
        var game2 = saves.First(s => s.SessionName == "Game 2");

        var loaded1 = await _service.LoadGameAsync(game1.Id.ToString());
        var loaded2 = await _service.LoadGameAsync(game2.Id.ToString());

        loaded1.SessionName.Should().Be("Game 1");
        loaded2.SessionName.Should().Be("Game 2");
    }
}

public class GameDataServiceEdgeCaseTests
{
    [Fact]
    public async Task CreateNewGameAsync_EmptyPlayerName_StillCreatesSession()
    {
        var factory = new TestGameDataContextFactory();
        var mockEventBus = new Mock<IEventBus>();
        var service = new TestableGameDataService();
        service.Initialize(factory, mockEventBus.Object);

        var settings = new GameSettings
        {
            SessionName = "Test",
            PlayerName = ""
        };

        await service.CreateNewGameAsync(settings);

        var saves = await service.GetAllSavesAsync();
        saves.Should().HaveCount(1);
    }

    [Fact]
    public async Task LoadGameAsync_EmptyString_ReturnsNull()
    {
        var factory = new TestGameDataContextFactory();
        var mockEventBus = new Mock<IEventBus>();
        var service = new TestableGameDataService();
        service.Initialize(factory, mockEventBus.Object);

        var result = await service.LoadGameAsync("");

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoadGameAsync_WhitespaceString_ReturnsNull()
    {
        var factory = new TestGameDataContextFactory();
        var mockEventBus = new Mock<IEventBus>();
        var service = new TestableGameDataService();
        service.Initialize(factory, mockEventBus.Object);

        var result = await service.LoadGameAsync("   ");

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateNewGameAsync_GeneratesUniqueIds()
    {
        var factory = new TestGameDataContextFactory();
        var mockEventBus = new Mock<IEventBus>();
        var service = new TestableGameDataService();
        service.Initialize(factory, mockEventBus.Object);

        var settings = TestDataBuilder.CreateDefaultGameSettings();

        await service.CreateNewGameAsync(settings);
        await service.CreateNewGameAsync(settings);

        var saves = await service.GetAllSavesAsync();
        saves.Should().HaveCount(2);
        saves[0].Id.Should().NotBe(saves[1].Id);
    }
}
