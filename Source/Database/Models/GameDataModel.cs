using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.UI.MenuControllers;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

[PrimaryKey("Id")]
public class GameSession
{
    [Key] public Guid Id { get; } = Guid.NewGuid();

    [Required] public string SessionName { get; set; } = "New Game";
    [Required] public string SaveName { get; set; } = "New Save";
    [Required] public string PlayerName { get; set; } = "New Player";

    public int InGameDay { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastSavedAt { get; set; } = null;

    // Game settings
    public int DifficultyLevel { get; set; }
    public int NumberOfAiPlayers { get; set; }
    public int NumberOfPlanets { get; set; }
    public StarType StarType { get; set; }
    public int StartingMineralModifier { get; set; }
    public int StartingEnergyModifier { get; set; }

    // Navigation properties
    [Required] public List<Guid> PlayerIds { get; set; } = new();
    [Required] public List<Player> Players { get; set; } = new();

    public Guid StrategicWorldStateId { get; set; }
    public StrategicWorldState StrategicWorldState { get; set; }

    // TODO: Tactical world state
    // public Guid? TacticalWorldStateId { get; set; }
    // public TacticalWorldState? TacticalWorldState { get; set; }
}