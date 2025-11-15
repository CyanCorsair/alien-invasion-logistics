using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Controllers.UI.NewGameScreen;
using Core.Database.Contexts;
using Core.Database.Models;
using Core.Types;
using Microsoft.EntityFrameworkCore;

namespace Core.Database
{
    [GlobalClass]
    public partial class GameDataService : Node, IGameDataService
    {
        private IGameDataContextFactory _contextFactory;

        public void Initialize(IGameDataContextFactory contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task CreateNewGameAsync(GameSettings settings)
        {
            await using var context = _contextFactory.CreateDbContext();

            GD.Print("Creating new game settings");

            GameSettingsModel newGameSettings = new()
            {
                Id = Guid.NewGuid(),
                PlayerName = settings.PlayerName,
                NumberOfPlanets = settings.NumberOfPlanets,
                StarType = settings.StarType,
                StartingEnergy = settings.StartingResources.energy,
                StartingMinerals = settings.StartingResources.minerals,
            };

            PlayerStateModel playerState = new()
            {
                Id = Guid.NewGuid(),
                ResourcesState = new ResourcesState
                {
                    EnergyStored = settings.StartingResources.energy,
                    MineralsStored = settings.StartingResources.minerals,
                    EnergyIncomeDaily = 0,
                    MineralsIncomeDaily = 0
                },
                ResearchState = new ResearchState
                {
                    CurrentResearch = null,
                    ResearchQueue = new List<ResearchItem>(),
                    CompletedResearch = settings.StartingResearch.startingResearch ?? new List<ResearchItem>()
                }
            };

            AIPlayerStateModel aiPlayerState = new()
            {
                Id = Guid.NewGuid(),
                ResourcesState = new ResourcesState
                {
                    EnergyStored = 1000,
                    MineralsStored = 1000,
                    EnergyIncomeDaily = 0,
                    MineralsIncomeDaily = 0
                },
                ResearchState = new ResearchState
                {
                    CurrentResearch = null,
                    ResearchQueue = new List<ResearchItem>(),
                    CompletedResearch = new List<ResearchItem>()
                }
            };

            SolarSystemState solarSystemState = new() { Id = Guid.NewGuid() };

            GameStateModel gameStateModel = new()
            {
                Id = Guid.NewGuid(),
                CurrentInGameDay = 0,
                CurrentTimeAcceleration = 1.0f,
                PlayerStateForeignKey = playerState.Id,
                AIPlayerStates = new List<AIPlayerStateModel> { aiPlayerState },
                SolarSystemStateForeignKey = solarSystemState.Id
            };

            GameDataModel newGameData = new()
            {
                Id = Guid.NewGuid(),
                GameSettingsForeignKey = newGameSettings.Id,
                GameStateForeignKey = gameStateModel.Id
            };

            context.GameSettings.Add(newGameSettings);
            context.PlayerState.Add(playerState);
            context.AIPLayerStates.Add(aiPlayerState);
            context.SolarSystemState.Add(solarSystemState);
            context.GameState.Add(gameStateModel);
            context.GameData.Add(newGameData);

            await context.SaveChangesAsync();
            GD.Print("New game created successfully");
        }

        public async Task<List<GameSaveModel>> GetAllSavesAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.GameSaves.ToListAsync();
        }

        public async Task<GameDataModel> LoadGameAsync(string saveId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var save = await context.GameSaves.FindAsync(Guid.Parse(saveId));
            if (save == null)
            {
                GD.PrintErr("No save found with that ID");
                return null;
            }

            var gameData = await context.GameData.FindAsync(save.GameDataForeignKey);
            if (gameData == null)
            {
                GD.PrintErr("No game data found for that save");
                return null;
            }

            return gameData;
        }

        public async Task SaveGameAsync(GameSaveData gameSave)
        {
            await using var context = _contextFactory.CreateDbContext();

            GameSaveModel newSave = new()
            {
                Id = Guid.NewGuid(),
                SaveGameName = gameSave.SaveName,
                InGameDay = gameSave.InGameDay,
                GameMode = gameSave.GameMode,
                GameDataForeignKey = gameSave.GameDataId,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            context.GameSaves.Add(newSave);
            await context.SaveChangesAsync();
        }

        public async Task ClearDatabaseAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
    }
}
