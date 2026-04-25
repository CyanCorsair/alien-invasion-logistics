#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlienInvasionLogistics.Source.Types;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

[PrimaryKey("Id")]
public class StrategicWorldState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Strategic World";
    public Guid SolarSystemId { get; set; }
    public SolarSystem SolarSystem { get; set; } = null!;
    public List<Nation> Nations { get; } = [];
    public List<Player> Players { get; } = [];
}

public class SolarSystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public BaseNaturalSolarObject CentralMass { get; set; } = null!;
    public List<PlanetarySystem> PlanetarySystems { get; set; } = [];
    public List<AsteroidBelt> AsteroidBelts { get; set; } = [];
    public List<CometCloud> CometClouds { get; set; } = [];
}

/// <summary>
/// An asteroid belt — an abstract orbital zone represented visually as a particle system.
/// Density drives the GPUParticles2D particle count.
/// </summary>
public class AsteroidBelt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Asteroid Belt";
    public float InnerRadius { get; set; }
    public float OuterRadius { get; set; }
    public int Density { get; set; }
    public int SunlightLevel { get; set; }
    public List<GameResource> ResourceDeposits { get; set; } = [];
}

/// <summary>
/// A comet cloud — an abstract outer-system zone (analogous to the Oort Cloud / Kuiper Belt).
/// Density drives the GPUParticles2D particle count.
/// </summary>
public class CometCloud
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Comet Cloud";
    public float InnerRadius { get; set; }
    public float OuterRadius { get; set; }
    public int Density { get; set; }
    public int SunlightLevel { get; set; }
    public List<GameResource> ResourceDeposits { get; set; } = [];
}

public class PlanetarySystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public BaseNaturalSolarObject CentralMass { get; set; } = null!;
    public List<BaseNaturalSolarObject>? CelestialBodies { get; set; }
}

public class BaseNaturalSolarObject : BaseObjectType
{
    public CelestialBodyType BodyType { get; set; } = CelestialBodyType.Planet;
    public bool IsMajorBody { get; set; } = true;
    // Physical properties
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Mass { get; set; }
    public float Radius { get; set; }
    // Orbital properties
    public float OrbitalPeriod { get; set; }
    public float SemiMajorAxis { get; set; }
    public float Eccentricity { get; set; }
    public float RotationPeriod { get; set; } = 24f; // Rotation period in hours
    // Resources and orbits
    public List<GameResource> ResourceDeposits { get; set; } = [];
    public int SunlightLevel { get; set; }
    public List<Orbit> Orbits { get; set; } = [];
    // Owned landing sites, possibly null based on type
    public List<LandingSite>? LandingSites { get; set; }
    // Orbital relationships
    public Guid? ParentBodyId { get; set; }
    public BaseNaturalSolarObject? ParentBody { get; set; }
    public List<BaseNaturalSolarObject>? ChildBodies { get; set; }
    public List<BaseNaturalSolarObject>? SiblingBodies { get; set; } // For bodies directly orbiting each other, e.g. binaries
}

// Base class for any location that can hold artificial objects (orbits and landing sites)
public abstract class ArtificialObjectContainer
{
    public int MaxStationaryArtificialObjects { get; set; }
    public int MaxMobileArtificialObjects { get; set; }
    public int CurrentStationaryArtificialObjects { get; set; }
    public int CurrentMobileArtificialObjects { get; set; }
    public List<StaticArtificialObject> StaticArtificialObjects { get; set; } = [];
    public List<MobileArtificialObject> MobileArtificialObjects { get; set; } = [];
}

[Owned]
public class Orbit : ArtificialObjectContainer
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Orbit 1";
    public OrbitType OrbitType { get; set; }
    public float Altitude { get; set; } // Altitude above body surface
    public float OrbitalRadius { get; set; } // Distance from body center
    public float OrbitalPeriodAtAltitude { get; set; } // Orbital period in hours
    public float OrbitalVelocity { get; set; } // Orbital velocity in km/s
}

[Owned]
public class LandingSite : ArtificialObjectContainer
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Landing Site 1";
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
    public Nation OwningNation { get; set; } = null!;
}

[PrimaryKey("Id")]
public class BaseObjectType
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Celestial Body";
}

public enum CelestialBodyType
{
    Star,
    GasGiant,       // >= 50 Earth masses (Jupiter/Saturn-like)
    IceGiant,       // 10-50 Earth masses (Uranus/Neptune-like)
    LargePlanet,
    Planet,
    DwarfPlanet,
    MinorPlanet,
    Moon,
    Asteroid,
    Comet
}