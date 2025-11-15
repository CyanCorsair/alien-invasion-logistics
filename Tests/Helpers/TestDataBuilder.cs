using Core.Database.Models;
using System;
using System.Collections.Generic;

namespace AlienInvasionLogistics.Tests.Helpers
{
    /// <summary>
    /// Builder class for creating test data objects
    /// </summary>
    public static class TestDataBuilder
    {
        public static PlayerStateModel CreateTestPlayerState()
        {
            return new PlayerStateModel
            {
                Id = Guid.NewGuid(),
                ResourcesState = new ResourcesState
                {
                    EnergyStored = 200,
                    MineralsStored = 2000,
                    EnergyIncomeDaily = 10,
                    MineralsIncomeDaily = 50
                },
                ResearchState = new ResearchState()
            };
        }

        public static AIPlayerStateModel CreateTestAIPlayerState()
        {
            return new AIPlayerStateModel
            {
                Id = Guid.NewGuid(),
                ResourcesState = new ResourcesState
                {
                    EnergyStored = 1000,
                    MineralsStored = 1000,
                    EnergyIncomeDaily = 5,
                    MineralsIncomeDaily = 25
                },
                ResearchState = new ResearchState()
            };
        }

        public static GameDataModel CreateTestGameData()
        {
            var playerState = CreateTestPlayerState();
            var aiPlayerState = CreateTestAIPlayerState();
            var solarSystemState = new SolarSystemState { Id = Guid.NewGuid() };

            var gameSettings = new GameSettingsModel
            {
                Id = Guid.NewGuid(),
                PlayerName = "Test Player",
                NumberOfPlanets = 5,
                StarType = 0, // MainSequence
                StartingEnergy = 200,
                StartingMinerals = 2000,
                StartingResearches = new List<string>()
            };

            var gameState = new GameStateModel
            {
                Id = Guid.NewGuid(),
                CurrentInGameDay = 0,
                CurrentTimeAcceleration = 1.0f,
                PlayerStateForeignKey = playerState.Id,
                PlayerState = playerState,
                AIPlayerStates = new List<AIPlayerStateModel> { aiPlayerState },
                SolarSystemStateForeignKey = solarSystemState.Id,
                SolarSystemState = solarSystemState
            };

            return new GameDataModel
            {
                Id = Guid.NewGuid(),
                GameSettingsForeignKey = gameSettings.Id,
                GameSettings = gameSettings,
                GameStateForeignKey = gameState.Id,
                GameState = gameState
            };
        }
    }
}
