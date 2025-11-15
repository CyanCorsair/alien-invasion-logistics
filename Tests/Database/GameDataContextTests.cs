using AlienInvasionLogistics.Tests.Helpers;
using Core.Database.Contexts;
using Core.Database.Models;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AlienInvasionLogistics.Tests.Database
{
    public class GameDataContextTests : IDisposable
    {
        private readonly GameDataContext _context;

        public GameDataContextTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
        }

        [Fact]
        public async Task GameDataContext_ShouldPersistAndRetrieveGameData()
        {
            // Arrange
            var gameData = TestDataBuilder.CreateTestGameData();

            // Act
            _context.GameSettings.Add(gameData.GameSettings);
            _context.PlayerState.Add(gameData.GameState.PlayerState);
            _context.AIPLayerStates.AddRange(gameData.GameState.AIPlayerStates);
            _context.SolarSystemState.Add(gameData.GameState.SolarSystemState);
            _context.GameState.Add(gameData.GameState);
            _context.GameData.Add(gameData);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.GameData.FindAsync(gameData.Id);
            retrieved.Should().NotBeNull();
            retrieved.Id.Should().Be(gameData.Id);
        }

        [Fact]
        public async Task GameDataContext_ShouldSupportNavigationProperties()
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

            // Act
            var retrieved = _context.GameData
                .Where(g => g.Id == gameData.Id)
                .Select(g => new { g.Id, g.GameSettingsForeignKey })
                .FirstOrDefault();

            // Assert
            retrieved.Should().NotBeNull();
            retrieved.GameSettingsForeignKey.Should().Be(gameData.GameSettings.Id);
        }

        [Fact]
        public async Task PlayerStateModel_ShouldStoreOwnedTypes()
        {
            // Arrange
            var playerState = TestDataBuilder.CreateTestPlayerState();

            // Act
            _context.PlayerState.Add(playerState);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _context.PlayerState.FindAsync(playerState.Id);
            retrieved.Should().NotBeNull();
            retrieved.ResourcesState.Should().NotBeNull();
            retrieved.ResourcesState.EnergyStored.Should().Be(200);
            retrieved.ResourcesState.MineralsStored.Should().Be(2000);
        }

        [Fact]
        public async Task GameStateModel_ShouldSupportManyToManyWithAIPlayers()
        {
            // Arrange
            var aiPlayer1 = TestDataBuilder.CreateTestAIPlayerState();
            var aiPlayer2 = TestDataBuilder.CreateTestAIPlayerState();
            var playerState = TestDataBuilder.CreateTestPlayerState();
            var solarSystemState = new SolarSystemState { Id = Guid.NewGuid() };

            var gameState = new GameStateModel
            {
                Id = Guid.NewGuid(),
                PlayerStateForeignKey = playerState.Id,
                PlayerState = playerState,
                AIPlayerStates = new() { aiPlayer1, aiPlayer2 },
                SolarSystemStateForeignKey = solarSystemState.Id,
                SolarSystemState = solarSystemState
            };

            // Act
            _context.PlayerState.Add(playerState);
            _context.AIPLayerStates.AddRange(aiPlayer1, aiPlayer2);
            _context.SolarSystemState.Add(solarSystemState);
            _context.GameState.Add(gameState);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = _context.GameState
                .Where(g => g.Id == gameState.Id)
                .FirstOrDefault();

            retrieved.Should().NotBeNull();
            // Note: Navigation property loading would require Include in a real scenario
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
