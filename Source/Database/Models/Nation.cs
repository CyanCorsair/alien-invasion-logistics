using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Models;

public class Nation
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Nation";

    [MaxLength(128)] public string FlagColorHex { get; set; } = "#FFFFFF";

    public Guid StrategicWorldStateId { get; set; }
    public StrategicWorldState StrategicWorldState { get; set; }

    public Guid PlayerId { get; set; }
    public Player Player { get; set; }

    public List<Guid> OccupiedCelestialBodyIds { get; set; } = new();
    public List<CelestialBody> OccupiedCelestialBodies { get; set; } = new();

    public List<StaticArtificialObject> OwnedStaticObjects { get; set; } = new();
    public List<MobileArtificialObject> OwnedMobileObjects { get; set; } = new();
    
    public StartingResearch StartingResearch { get; set; }
    public StartingResources StartingResources { get; set; }
}