using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Dtos;

/// <summary>
/// Data transfer object for star game objects.
/// </summary>
public class StarGameObjectDto : IGameObjectDto
{
    // Identity
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Physical properties
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Mass { get; set; }
    public float Radius { get; set; }

    // Star-specific
    public CelestialBodyType BodyType { get; set; } // Should be Star

    // Orbital properties (for binary stars)
    public float OrbitalPeriod { get; set; }
    public float SemiMajorAxis { get; set; }
    public float Eccentricity { get; set; }

    // Relationships (binary star systems)
    public List<Guid> SiblingBodyIds { get; set; } = new();

    // Game data
    public List<GameResource> ResourceDeposits { get; set; } = new();
    public List<Orbit> Orbits { get; set; } = new();
}
