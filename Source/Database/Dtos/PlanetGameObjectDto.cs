using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Dtos;

/// <summary>
/// Data transfer object for planet game objects.
/// </summary>
public class PlanetGameObjectDto : IGameObjectDto
{
    // Identity
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Physical properties
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Mass { get; set; }
    public float Radius { get; set; }

    // Orbital properties
    public float OrbitalPeriod { get; set; }
    public float SemiMajorAxis { get; set; }
    public float Eccentricity { get; set; }

    // Gameplay properties
    public int SunlightLevel { get; set; }
    public CelestialBodyType BodyType { get; set; }

    // Relationships
    public Guid? ParentBodyId { get; set; }

    // Game data
    public List<GameResource> ResourceDeposits { get; set; } = new();
    public List<Orbit> Orbits { get; set; } = new();
    public List<LandingSite> LandingSites { get; set; } = new();
}
