using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Database.Models
{
    public class GameDataModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Navigation properties instead of just foreign keys
        public Guid GameSettingsForeignKey { get; set; }
        public GameSettingsModel GameSettings { get; set; }

        public Guid GameStateForeignKey { get; set; }
        public GameStateModel GameState { get; set; }
    }

    public class GameSettingsModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberOfPlanets { get; set; }
        public int StarType { get; set; }
        public int StartingMinerals { get; set; }
        public int StartingEnergy { get; set; }
        public List<string> StartingResearches { get; set; }
        public string PlayerName { get; set; } = "Default";
    }

    public class GameSaveModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string SaveGameName { get; set; } = "New Save";

        public Guid GameDataForeignKey { get; set; }
        public GameDataModel GameData { get; set; }

        public int InGameDay { get; set; } = 0;
        public int GameMode { get; set; } = 0; // 0 = Strat, 1 = Tac
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class GameStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public float CurrentTimeAcceleration { get; set; } = 1.0f;
        public int CurrentInGameDay { get; set; } = 0;

        public Guid PlayerStateForeignKey { get; set; }
        public PlayerStateModel PlayerState { get; set; }

        // Many-to-many relationship with AI players (managed by junction table)
        public List<AIPlayerStateModel> AIPlayerStates { get; set; } = new();

        public Guid SolarSystemStateForeignKey { get; set; }
        public SolarSystemState SolarSystemState { get; set; }

        public int GameMode { get; set; } = 0; // 0 = Strat, 1 = Tac
    }
}
