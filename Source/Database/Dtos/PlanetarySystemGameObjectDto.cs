using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Dtos;

/// <summary>
/// Data transfer object for planetary system game objects.
/// Represents a planet and its moons/satellites.
/// </summary>
public class PlanetarySystemGameObjectDto : IGameObjectDto
{
    // Identity
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Central body (the planet)
    public Guid CentralMassId { get; set; }

    // Collections (moons, minor bodies, nested systems)
    public List<Guid> CelestialBodyIds { get; set; } = new();
    public List<Guid> PlanetarySystemIds { get; set; } = new();
}
