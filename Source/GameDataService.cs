using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.Services;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.UI.MenuControllers;
using AlienInvasionLogistics.Source.Utilities;
using Godot;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

#nullable enable

namespace AlienInvasionLogistics.Source;

public partial class GameDataService : Node, IGameDataService
{
    private IGameDataContextFactory _contextFactory;
    private IEventBus _eventBus;

    public async Task CreateNewGameAsync(GameSettings settings)
    {
        await using var context = _contextFactory.CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ErrorHandler.LogMessage("Creating new game", severity: ErrorUtilities.MessageLevel.Info);

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

            SolarSystemGenerator generator = new SolarSystemGenerator();
            StartingSettings startingStratSettings = new StartingSettings()
            {
                GameName = gameSession.SessionName,
                Difficulty = (GameDifficulty)gameSession.DifficultyLevel
            };
            StrategicWorldState initialWorldState = new();
            SolarSystem solarSystemDefinition = generator.GenerateSolarSystemState(startingStratSettings);
            initialWorldState.SolarSystem = solarSystemDefinition;
            initialWorldState.SolarSystemId = solarSystemDefinition.Id;
            
            

            ErrorHandler.LogMessage($"{JsonConvert.SerializeObject(
                gameSession, Formatting.Indented)}",
                severity: ErrorUtilities.MessageLevel.Info);
            context.GameSessions.Add(gameSession);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            ErrorHandler.LogMessage("New game created successfully", severity: ErrorUtilities.MessageLevel.Info);
            _eventBus.Publish(new GameCreatedEvent(gameSession.Id, settings.PlayerName));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            ErrorHandler.LogMessage("Failed to create new game", ex);
            throw;
        }
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

    public void Initialize(IGameDataContextFactory contextFactory, IEventBus eventBus)
    {
        _contextFactory =
            contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
    }

    public async Task<List<GameSession>> GetAllSavesAsync()
    {
        await using var context = _contextFactory.CreateDbContext();
        return await context.GameSessions.ToListAsync();
    }

    public async Task<GameSession?> LoadGameAsync(Guid saveId)
    {
        await using var context = _contextFactory.CreateDbContext();

        var save = await context.GameSessions
            .Include(gs => gs.Players)
                .ThenInclude(p => p.Nation)
            .Include(gs => gs.Players)
                .ThenInclude(p => p.ResearchState)
            .Include(gs => gs.Players)
                .ThenInclude(p => p.ResourceState)
            .Include(gs => gs.StrategicWorldState)
                .ThenInclude(sws => sws.SolarSystem)
                    .ThenInclude(ss => ss.CentralMass)
            .Include(gs => gs.StrategicWorldState)
                .ThenInclude(sws => sws.SolarSystem)
                    .ThenInclude(ss => ss.PlanetarySystems)
            .FirstOrDefaultAsync(gs => gs.Id == saveId);

        if (save != null)
        {
            _eventBus.Publish(new GameLoadedEvent(save.Id, save.SessionName));
            return save;
        }

        ErrorHandler.LogMessage("No save found with that ID", severity: ErrorUtilities.MessageLevel.Warning);
        return null;
    }
}