using Core.Database.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AlienInvasionLogistics.Tests.Models
{
    public class DatabaseModelTests
    {
        [Fact]
        public void GameDataModel_ShouldInitializeWithNewGuid()
        {
            // Act
            var model = new GameDataModel();

            // Assert
            model.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void GameSettingsModel_ShouldStorePlayerName()
        {
            // Arrange
            var model = new GameSettingsModel();

            // Act
            model.PlayerName = "Test Player";

            // Assert
            model.PlayerName.Should().Be("Test Player");
        }

        [Fact]
        public void GameSettingsModel_ShouldStoreStartingResources()
        {
            // Arrange
            var model = new GameSettingsModel();

            // Act
            model.StartingEnergy = 300;
            model.StartingMinerals = 3000;
            model.NumberOfPlanets = 7;
            model.StarType = 1;

            // Assert
            model.StartingEnergy.Should().Be(300);
            model.StartingMinerals.Should().Be(3000);
            model.NumberOfPlanets.Should().Be(7);
            model.StarType.Should().Be(1);
        }

        [Fact]
        public void PlayerStateModel_ShouldStoreResourcesState()
        {
            // Arrange
            var model = new PlayerStateModel();
            var resourcesState = new ResourcesState
            {
                EnergyStored = 200,
                MineralsStored = 2000,
                EnergyIncomeDaily = 10,
                MineralsIncomeDaily = 50
            };

            // Act
            model.ResourcesState = resourcesState;

            // Assert
            model.ResourcesState.Should().NotBeNull();
            model.ResourcesState.EnergyStored.Should().Be(200);
            model.ResourcesState.MineralsStored.Should().Be(2000);
        }

        [Fact]
        public void AIPlayerStateModel_ShouldStoreResourcesState()
        {
            // Arrange
            var model = new AIPlayerStateModel();
            var resourcesState = new ResourcesState
            {
                EnergyStored = 1000,
                MineralsStored = 1000
            };

            // Act
            model.ResourcesState = resourcesState;

            // Assert
            model.ResourcesState.Should().NotBeNull();
            model.ResourcesState.EnergyStored.Should().Be(1000);
        }

        [Fact]
        public void GameStateModel_ShouldStoreAIPlayerStates()
        {
            // Arrange
            var model = new GameStateModel();
            var aiPlayer1 = new AIPlayerStateModel { Id = Guid.NewGuid() };
            var aiPlayer2 = new AIPlayerStateModel { Id = Guid.NewGuid() };

            // Act
            model.AIPlayerStates = new List<AIPlayerStateModel> { aiPlayer1, aiPlayer2 };

            // Assert
            model.AIPlayerStates.Should().HaveCount(2);
            model.AIPlayerStates.Should().Contain(aiPlayer1);
            model.AIPlayerStates.Should().Contain(aiPlayer2);
        }

        [Fact]
        public void GameSaveModel_ShouldInitializeWithDefaults()
        {
            // Act
            var model = new GameSaveModel();

            // Assert
            model.SaveGameName.Should().Be("New Save");
            model.InGameDay.Should().Be(0);
            model.GameMode.Should().Be(0);
            model.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void SolarSystemState_ShouldStoreNavigationProperties()
        {
            // Arrange
            var model = new SolarSystemState();
            var sun = new SunStateModel { Id = Guid.NewGuid(), DisplayName = "Test Sun" };
            var planetarySystem = new PlanetarySystemState { Id = Guid.NewGuid(), Name = "Inner System" };

            // Act
            model.SystemSun = sun;
            model.PlanetarySystems = new List<PlanetarySystemState> { planetarySystem };

            // Assert
            model.SystemSun.Should().NotBeNull();
            model.SystemSun.DisplayName.Should().Be("Test Sun");
            model.PlanetarySystems.Should().HaveCount(1);
        }

        [Fact]
        public void PlanetStateModel_ShouldStorePhysicsProperties()
        {
            // Arrange
            var model = new PlanetStateModel();

            // Act
            model.DisplayName = "Earth";
            model.PositionX = 100f;
            model.PositionY = 200f;
            model.VelocityX = 5f;
            model.VelocityY = 10f;
            model.Mass = 5.972e24f;

            // Assert
            model.DisplayName.Should().Be("Earth");
            model.PositionX.Should().Be(100f);
            model.PositionY.Should().Be(200f);
            model.VelocityX.Should().Be(5f);
            model.VelocityY.Should().Be(10f);
            model.Mass.Should().Be(5.972e24f);
        }
    }
}
