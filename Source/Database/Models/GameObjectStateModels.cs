using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

public class StrategicWorldState
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Strategic World";
    public Guid SolarSystemId { get; set; }
    public List<Guid> NationIds { get; set; } = new();
    public List<Guid> PlayerIds { get; set; } = new();
    public SolarSystem SolarSystem { get; set; }
    public List<Nation> Nations { get; set; } = new();
    public List<Player> Players { get; set; } = new();
}

public class SolarSystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public CelestialBody CentralMass { get; set; }
    public List<Guid> PlanetarySystemIds { get; set; }
    public List<PlanetarySystem> PlanetarySystems { get; set; } = new(); // Top level list of system
}

public class PlanetarySystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public CelestialBody CentralMass { get; set; }
    #nullable enable
    public List<Guid>? CelestialBodyIds { get; set; }
    public List<CelestialBody>? CelestialBodies { get; set; }
    public List<Guid>? PlanetarySystemIds { get; set; }
    public List<PlanetarySystem>? PlanetarySystems { get; set; }
    #nullable disable
}

public class CelestialBody : BaseObjectType
{
    public CelestialBodyType BodyType { get; set; } = CelestialBodyType.Planet;
    // Physical properties
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Mass { get; set; }
    public float Radius { get; set; }
    // Orbital properties
    public float OrbitalPeriod { get; set; }
    public float SemiMajorAxis { get; set; }
    public float Eccentricity { get; set; }
    // Resources and orbits
    public List<GameResource> ResourceDeposits { get; set; } = new();
    public int SunlightLevel { get; set; }
    public List<Orbit> Orbits { get; set; } = new();
    #nullable enable
    // Owned landing sites, possibly null based on type
    public List<LandingSite>? LandingSites { get; set; }
    // Orbital relationships
    public Guid? ParentBodyId { get; set; }
    public List<Guid>? ChildBodyIds { get; set; }
    public List<Guid>? SiblingBodyIds { get; set; }
    public CelestialBody? ParentBody { get; set; }
    public List<CelestialBody>? ChildBodies { get; set; }
    public List<CelestialBody>? SiblingBodies { get; set; } // For bodies directly orbiting each other, e.g. binaries
    // Occupation
    public Guid? OccupyingNationId { get; set; }
    #nullable disable
}

[Owned]
public class Orbit
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Orbit 1";
    public int MaxStationaryArtificialObjects { get; set; }
    public int MaxMobileArtificialObjects { get; set; }
    public int CurrentStationaryArtificialObjects { get; set; }
    public int CurrentMobileArtificialObjects { get; set; }
    public List<Guid> StaticArtificialObjectIds { get; set; } = new();
    public List<StaticArtificialObject> StaticArtificialObjects { get; set; } = new();
    public List<Guid> MobileArtificialObjectIds { get; set; } = new();
    public List<MobileArtificialObject> MobileArtificialObjects { get; set; } = new();
}

[Owned]
public class LandingSite
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Landing Site 1";
    public int MaxStationaryArtificialObjects { get; set; }
    public int MaxMobileArtificialObjects { get; set; }
    public int CurrentStationaryArtificialObjects { get; set; }
    public int CurrentMobileArtificialObjects { get; set; }
    public List<Guid> StaticArtificialObjectIds { get; set; } = new();
    public List<StaticArtificialObject> StaticArtificialObjects { get; set; } = new();
    public List<Guid> MobileArtificialObjectIds { get; set; } = new();
    public List<MobileArtificialObject> MobileArtificialObjects { get; set; } = new();
}

public class StaticArtificialObject : BaseArtificialObject
{
}

public class MobileArtificialObject : BaseArtificialObject
{
}

public class BaseArtificialObject : BaseObjectType
{
    public Guid OwningNationId { get; set; }
    public Nation OwningNation { get; set; }
}

public class BaseObjectType
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Celestial Body";
}

public enum CelestialBodyType
{
    Star,
    GiantPlanet,
    LargePlanet,
    Planet,
    DwarfPlanet,
    MinorPlanet,
    Moon,
    Asteroid,
    Comet
}