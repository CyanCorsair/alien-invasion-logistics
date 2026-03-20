using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlienInvasionLogistics.Source.Types;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

[PrimaryKey("Id")]
public class StrategicWorldState
{
    [Key] public Guid Id { get; } = Guid.NewGuid();
    [Required][MaxLength(128)] public string Name { get; set; } = "Unnamed Strategic World";
    public Guid SolarSystemId { get; set; }
    public List<Guid> NationIds { get; } = [];
    public List<Guid> PlayerIds { get; } = [];
    public SolarSystem SolarSystem { get; set; }
    public List<Nation> Nations { get; } = [];
    public List<Player> Players { get; } = [];
}

public class SolarSystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public BaseNaturalSolarObject CentralMass { get; set; }
    public List<Guid> PlanetarySystemIds { get; set; }
    public List<PlanetarySystem> PlanetarySystems { get; set; } = []; // Top level list of system
}

public class PlanetarySystem : BaseObjectType
{
    public Guid CentralMassId { get; set; }
    public BaseNaturalSolarObject CentralMass { get; set; }
    #nullable enable
    public List<Guid>? CelestialBodyIds { get; set; }
    public List<BaseNaturalSolarObject>? CelestialBodies { get; set; }
    public List<Guid>? PlanetarySystemIds { get; set; }
    public List<PlanetarySystem>? PlanetarySystems { get; set; }
    #nullable disable
}

public class BaseNaturalSolarObject : BaseObjectType
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
    public float RotationPeriod { get; set; } = 24f; // Rotation period in hours
    // Resources and orbits
    public List<GameResource> ResourceDeposits { get; set; } = [];
    public int SunlightLevel { get; set; }
    public List<Orbit> Orbits { get; set; } = [];
    #nullable enable
    // Owned landing sites, possibly null based on type
    public List<LandingSite>? LandingSites { get; set; }
    // Orbital relationships
    public Guid? ParentBodyId { get; set; }
    public List<Guid>? ChildBodyIds { get; set; }
    public List<Guid>? SiblingBodyIds { get; set; }
    public BaseNaturalSolarObject? ParentBody { get; set; }
    public List<BaseNaturalSolarObject>? ChildBodies { get; set; }
    public List<BaseNaturalSolarObject>? SiblingBodies { get; set; } // For bodies directly orbiting each other, e.g. binaries
    // Occupation
    public List<Guid>? OccupyingNationIds { get; set; }
    #nullable disable
}

[Owned]
public class Orbit
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Orbit 1";
    public OrbitType OrbitType { get; set; }
    public float Altitude { get; set; } // Altitude above body surface
    public float OrbitalRadius { get; set; } // Distance from body center
    public float OrbitalPeriodAtAltitude { get; set; } // Orbital period in hours
    public float OrbitalVelocity { get; set; } // Orbital velocity in km/s
    public int MaxStationaryArtificialObjects { get; set; }
    public int MaxMobileArtificialObjects { get; set; }
    public int CurrentStationaryArtificialObjects { get; set; }
    public int CurrentMobileArtificialObjects { get; set; }
    public List<Guid> StaticArtificialObjectIds { get; set; } = [];
    public List<StaticArtificialObject> StaticArtificialObjects { get; set; } = [];
    public List<Guid> MobileArtificialObjectIds { get; set; } = [];
    public List<MobileArtificialObject> MobileArtificialObjects { get; set; } = [];
}

[Owned]
public class LandingSite
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Landing Site 1";
    public int MaxStationaryArtificialObjects { get; set; }
    public int MaxMobileArtificialObjects { get; set; }
    public int CurrentStationaryArtificialObjects { get; set; }
    public int CurrentMobileArtificialObjects { get; set; }
    public List<Guid> StaticArtificialObjectIds { get; set; } = [];
    public List<StaticArtificialObject> StaticArtificialObjects { get; set; } = [];
    public List<Guid> MobileArtificialObjectIds { get; set; } = [];
    public List<MobileArtificialObject> MobileArtificialObjects { get; set; } = [];
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

[PrimaryKey("Id")]
public class BaseObjectType
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
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