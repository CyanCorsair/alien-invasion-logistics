using AlienInvasionLogistics.Tests.Helpers;
using Core.Database.Contexts;
using Core.Database.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AlienInvasionLogistics.Tests.Database
{
    /// <summary>
    /// Tests for database operations logic (without Godot dependencies)
    /// </summary>
    public class GameDataServiceTests : IDisposable
    {
        private readonly GameDataContext _context;

        public GameDataServiceTests()
        {
            var options = new DbContextOptionsBuilder<GameDataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new GameDataContext(options);
        }

        [Fact]
        public async Task CreateNewGame_ShouldPersistAllEntities()
        {
            // Arrange
            var gameSettings = new GameSettingsModel
            {
                PlayerName = "Test Player",
                NumberOfPlanets = 5,
                StarType = 0,
                StartingEnergy = 200,
                StartingMinerals = 2000,
                StartingResearches = new List<string>()
            };

            var playerState = new PlayerStateModel
            {
                ResourcesState = new ResourcesState
                {
                    EnergyStored = 200,
                    MineralsStored = 2000
                },
                ResearchState = new ResearchState()
            };

            var aiPlayerState = new AIPlayerStateModel
            {
                ResourcesState = new ResourcesState { EnergyStored = 1000, MineralsStored = 1000 },
                ResearchState = new ResearchState()
            };

            var solarSystemState = new SolarSystemState();

            var gameState = new GameStateModel
            {
                PlayerStateForeignKey = playerState.Id,
                PlayerState = playerState,
                AIPlayerStates = new List<AIPlayerStateModel> { aiPlayerState },
                SolarSystemStateForeignKey = solarSystemState.Id,
                SolarSystemState = solarSystemState
            };

            var gameData = new GameDataModel
            {
                GameSettingsForeignKey = gameSettings.Id,
                GameSettings = gameSettings,
                GameStateForeignKey = gameState.Id,
                GameState = gameState
            };

            // Act
            _context.GameSettings.Add(gameSettings);
            _context.PlayerState.Add(playerState);
            _context.AIPLayerStates.Add(aiPlayerState);
            _context.SolarSystemState.Add(solarSystemState);
            _context.GameState.Add(gameState);
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();

            // Assert
            var savedGameData = await _context.GameData.FirstOrDefaultAsync();
            savedGameData.Should().NotBeNull();

            var savedSettings = await _context.GameSettings.FirstOrDefaultAsync();
            savedSettings.Should().NotBeNull();
            savedSettings.PlayerName.Should().Be("Test Player");
            savedSettings.StartingEnergy.Should().Be(200);
        }

        [Fact]
        public async Task GameSave_ShouldStoreMetadata()
        {
            // Arrange
            var gameData = TestDataBuilder.CreateTestGameData();
            _context.GameSettings.Add(gameData.GameSettings);
            _context.PlayerState.Add(gameData.GameState.PlayerState);
            _context.AIPLayerStates.AddRange(gameData.GameState.AIPlayerStates);
            _context.SolarSystemState.Add(gameData.GameState.SolarSystemState);
            _context.GameState.Add(gameData.GameState);
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();

            var gameSave = new GameSaveModel
            {
                SaveGameName = "Test Save",
                InGameDay = 42,
                GameMode = 0,
                GameDataForeignKey = gameData.Id,
                GameData = gameData
            };

            // Act
            _context.GameSaves.Add(gameSave);
            await _context.SaveChangesAsync();

            // Assert
            var savedSave = await _context.GameSaves.FirstOrDefaultAsync();
            savedSave.Should().NotBeNull();
            savedSave.SaveGameName.Should().Be("Test Save");
            savedSave.InGameDay.Should().Be(42);
            savedSave.GameDataForeignKey.Should().Be(gameData.Id);
        }

        [Fact]
        public async Task GetAllSaves_ShouldReturnAllSaves()
        {
            // Arrange - create 3 saves
            for (int i = 0; i < 3; i++)
            {
                var gameData = TestDataBuilder.CreateTestGameData();
                _context.GameSettings.Add(gameData.GameSettings);
                _context.PlayerState.Add(gameData.GameState.PlayerState);
                _context.AIPLayerStates.AddRange(gameData.GameState.AIPlayerStates);
                _context.SolarSystemState.Add(gameData.GameState.SolarSystemState);
                _context.GameState.Add(gameData.GameState);
                _context.GameData.Add(gameData);

                _context.GameSaves.Add(new GameSaveModel
                {
                    SaveGameName = $"Save {i}",
                    GameDataForeignKey = gameData.Id
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var saves = await _context.GameSaves.ToListAsync();

            // Assert
            saves.Should().HaveCount(3);
        }

        [Fact]
        public async Task ClearDatabase_ShouldRemoveAllEntities()
        {
            // Arrange
            var gameData = TestDataBuilder.CreateTestGameData();
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();

            // Act
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            // Assert
            var remaining = await _context.GameData.ToListAsync();
            remaining.Should().BeEmpty();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
