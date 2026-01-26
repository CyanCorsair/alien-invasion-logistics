using System;
using System.Collections.Generic;
using System.Threading;
using AlienInvasionLogistics.Source.Constants;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.Interfaces;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.Utilities;
using Godot;

namespace AlienInvasionLogistics.Source.Services;

/// <summary>
///     Generates solar systems based on game settings
/// </summary>
[GlobalClass]
public partial class SolarSystemGenerator : Node, ISolarSystemGenerator
{
    // TODO: Implement procedural planet name generation using this array
    private static readonly string[] PLANET_NAMES =
    {
        "Mercury",
        "Venus",
        "Earth",
        "Mars",
        "Jupiter",
        "Saturn",
        "Uranus",
        "Neptune",
        "Pluto",
        "Eris"
    };

    // Thread-safe random number generator using ThreadLocal for concurrent generation support
    private ThreadLocal<Random> _random;
    private int? _seed;

    private IEventBus _eventBus;
    
    /// <summary>
    /// Initializes the service with required dependencies.
    /// </summary>
    /// <param name="eventBus">The event bus for publishing generation events.</param>
    /// <param name="seed">Optional int for setting up the seed.</param>

    public void Initialize(IEventBus eventBus, int? seed = null)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        
        _seed = seed ?? (int)new Random().NextDouble() * 1_000_000;
        _random = new ThreadLocal<Random>(() =>
            seed.HasValue ? new Random(seed.Value) : new Random()
        );
    }

    public SolarSystem GenerateSolarSystemState(IStartingSettings settings)
    {
        ErrorHandler.LogMessage($"Generating solar system with {settings.CelestialBodyCount} planets", severity: ErrorUtilities.MessageLevel.Info);

        var solarSystem = new SolarSystem
        {
            Id = Guid.NewGuid(),
            Name = settings.GameName ?? "Solar System"
        };

        // Phase 1: Create central star
        var star = GenerateCentralStar(settings.CentralSolarMass);
        solarSystem.CentralMass = star;
        solarSystem.CentralMassId = star.Id;

        // Phase 2: Generate planetary systems
        solarSystem.PlanetarySystems = new List<PlanetarySystem>();
        solarSystem.PlanetarySystemIds = new List<Guid>();

        int planetCount = Math.Min(settings.CelestialBodyCount, SolarSystemConstants.MAX_PLANETS);
        float previousDistance = 0f;

        for (int i = 0; i < planetCount; i++)
        {
            var planetarySystem = GeneratePlanetarySystem(i, planetCount, star.Id, ref previousDistance);
            solarSystem.PlanetarySystems.Add(planetarySystem);
            solarSystem.PlanetarySystemIds.Add(planetarySystem.Id);
        }

        ErrorHandler.LogMessage($"Solar system generated: {planetCount} planets", severity: ErrorUtilities.MessageLevel.Info);
        _eventBus?.Publish(new SolarSystemGeneratedEvent(solarSystem.Id, planetCount));

        return solarSystem;
    }

    private CelestialBody GenerateCentralStar(Star starSettings)
    {
        var star = new CelestialBody
        {
            Id = Guid.NewGuid(),
            Name = starSettings.DisplayName ?? "Sol",
            BodyType = CelestialBodyType.Star,
            PositionX = 0f,
            PositionY = 0f,
            Mass = starSettings.Mass > 0 ? starSettings.Mass : 1.0f,
            Radius = 50f,
            OrbitalPeriod = 0f,
            SemiMajorAxis = 0f,
            Eccentricity = 0f,
            SunlightLevel = 100,
            Orbits = GenerateOrbitalZones(3, 5),
            ResourceDeposits = new List<GameResource>()
        };

        return star;
    }

    private PlanetarySystem GeneratePlanetarySystem(int index, int totalPlanets, Guid starId, ref float previousDistance)
    {
        var bodyType = DetermineBodyType(index, totalPlanets);
        string planetName = index < PLANET_NAMES.Length ? PLANET_NAMES[index] : $"Planet {index + 1}";

        float distance = CalculateOrbitalDistance(index, ref previousDistance);

        var planet = GeneratePlanet(planetName, bodyType, distance, starId);

        var system = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = $"{planetName} System",
            CentralMass = planet,
            CentralMassId = planet.Id,
            CelestialBodies = new List<CelestialBody>(),
            CelestialBodyIds = new List<Guid>()
        };

        if ((bodyType == CelestialBodyType.LargePlanet || bodyType == CelestialBodyType.GiantPlanet)
            && _random.Value.NextDouble() < 0.25)
        {
            int moonCount = _random.Value.Next(1, 4);
            for (int m = 0; m < moonCount; m++)
            {
                var moon = GenerateMoon(m, planet.Id, planet.PositionX, planet.PositionY);
                system.CelestialBodies.Add(moon);
                system.CelestialBodyIds.Add(moon.Id);
            }
        }

        return system;
    }

    private CelestialBody GeneratePlanet(string name, CelestialBodyType bodyType, float distance, Guid parentId)
    {
        float mass = GenerateMass(bodyType);
        float radius = CalculateRadius(mass, bodyType);
        float eccentricity = GenerateEccentricity();
        float orbitalPeriod = CalculateOrbitalPeriod(distance);

        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);
        float posX = distance * MathF.Cos(angle);
        float posY = distance * MathF.Sin(angle);

        int sunlightLevel = (int)(100 * (1.0f / (1.0f + distance / SolarSystemConstants.BASE_ORBITAL_RADIUS)));

        var planet = new CelestialBody
        {
            Id = Guid.NewGuid(),
            Name = name,
            BodyType = bodyType,
            PositionX = posX,
            PositionY = posY,
            Mass = mass,
            Radius = radius,
            OrbitalPeriod = orbitalPeriod,
            SemiMajorAxis = distance,
            Eccentricity = eccentricity,
            ParentBodyId = parentId,
            SunlightLevel = sunlightLevel,
            ResourceDeposits = GenerateResources(bodyType, distance),
            Orbits = GenerateOrbitalZones(2, 4),
            LandingSites = bodyType != CelestialBodyType.GiantPlanet ? GenerateLandingSites(1, 3) : null
        };

        return planet;
    }

    private CelestialBody GenerateMoon(int index, Guid parentPlanetId, float planetX, float planetY)
    {
        float moonDistance = 20f + (index * 15f);
        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);

        var moon = new CelestialBody
        {
            Id = Guid.NewGuid(),
            Name = $"Moon {index + 1}",
            BodyType = CelestialBodyType.Moon,
            PositionX = planetX + moonDistance * MathF.Cos(angle),
            PositionY = planetY + moonDistance * MathF.Sin(angle),
            Mass = (float)(_random.Value.NextDouble() * 0.1 + 0.01),
            Radius = (float)(_random.Value.NextDouble() * 5 + 2),
            OrbitalPeriod = CalculateOrbitalPeriod(moonDistance) * 0.1f,
            SemiMajorAxis = moonDistance,
            Eccentricity = (float)(_random.Value.NextDouble() * 0.05),
            ParentBodyId = parentPlanetId,
            SunlightLevel = 50,
            ResourceDeposits = GenerateResources(CelestialBodyType.Moon, moonDistance),
            Orbits = GenerateOrbitalZones(1, 2),
            LandingSites = GenerateLandingSites(1, 2)
        };

        return moon;
    }

    private CelestialBodyType DetermineBodyType(int index, int totalPlanets)
    {
        double position = (double)index / totalPlanets;

        if (position < 0.4)
            return _random.Value.NextDouble() < 0.7 ? CelestialBodyType.Planet : CelestialBodyType.LargePlanet;

        if (position < 0.7)
            return _random.Value.NextDouble() < 0.5 ? CelestialBodyType.LargePlanet : CelestialBodyType.GiantPlanet;

        return _random.Value.NextDouble() < 0.6 ? CelestialBodyType.GiantPlanet : CelestialBodyType.DwarfPlanet;
    }

    private float CalculateOrbitalDistance(int index, ref float previousDistance)
    {
        float baseDistance = SolarSystemConstants.BASE_ORBITAL_RADIUS * MathF.Pow(1.7f, index);

        float variance = (float)(_random.Value.NextDouble() - 0.5) * SolarSystemConstants.ORBITAL_RADIUS_VARIANCE;
        float distance = baseDistance + variance;

        if (distance < previousDistance + 50f)
            distance = previousDistance + 50f;

        previousDistance = distance;
        return distance;
    }

    private float GenerateMass(CelestialBodyType bodyType)
    {
        return bodyType switch
        {
            CelestialBodyType.DwarfPlanet => (float)(_random.Value.NextDouble() * 0.5 + 0.01),
            CelestialBodyType.Planet => (float)(_random.Value.NextDouble() * 2 + 0.5),
            CelestialBodyType.LargePlanet => (float)(_random.Value.NextDouble() * 5 + 2),
            CelestialBodyType.GiantPlanet => (float)(_random.Value.NextDouble() * 200 + 50),
            CelestialBodyType.Moon => (float)(_random.Value.NextDouble() * 0.1 + 0.01),
            _ => 1.0f
        };
    }

    private float CalculateRadius(float mass, CelestialBodyType bodyType)
    {
        float multiplier = bodyType == CelestialBodyType.GiantPlanet ? 3.0f : 1.0f;
        return MathF.Pow(mass, 1.0f / 3.0f) * 10f * multiplier;
    }

    private float GenerateEccentricity()
    {
        double roll = _random.Value.NextDouble();
        if (roll < 0.7)
            return (float)(_random.Value.NextDouble() * 0.05);
        else if (roll < 0.95)
            return (float)(_random.Value.NextDouble() * 0.1 + 0.05);
        else
            return (float)(_random.Value.NextDouble() * 0.15 + 0.15);
    }

    private float CalculateOrbitalPeriod(float semiMajorAxis)
    {
        // Prevent division by zero
        if (!(SolarSystemConstants.BASE_ORBITAL_RADIUS <= 0))
            return MathF.Sqrt(MathF.Pow(semiMajorAxis / SolarSystemConstants.BASE_ORBITAL_RADIUS, 3))
                   * SolarSystemConstants.ORBITAL_SPEED_BASE;
    }

    private List<GameResource> GenerateResources(CelestialBodyType bodyType, float distance)
    {
        var resources = new List<GameResource>();
        int resourceCount = _random.Value.Next(0, 4);

        for (int i = 0; i < resourceCount; i++)
        {
            bool isMinerals = distance < SolarSystemConstants.BASE_ORBITAL_RADIUS * 3
                ? _random.Value.NextDouble() < 0.7
                : _random.Value.NextDouble() < 0.3;

            resources.Add(new GameResource
            {
                Name = isMinerals ? "Minerals" : "Energy",
                ResourceType = isMinerals ? ResourceTypes.Minerals : ResourceTypes.Energy,
                Quantity = _random.Value.Next(100, 1000),
                MaxQuantity = _random.Value.Next(5000, 20000)
            });
        }

        return resources;
    }

    private List<Orbit> GenerateOrbitalZones(int min, int max)
    {
        var orbits = new List<Orbit>();
        int count = _random.Value.Next(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            orbits.Add(new Orbit
            {
                Name = $"Orbit {i + 1}",
                MaxStationaryArtificialObjects = _random.Value.Next(5, 20),
                MaxMobileArtificialObjects = _random.Value.Next(10, 50),
                CurrentStationaryArtificialObjects = 0,
                CurrentMobileArtificialObjects = 0,
                StaticArtificialObjects = new List<StaticArtificialObject>(),
                MobileArtificialObjects = new List<MobileArtificialObject>()
            });
        }

        return orbits;
    }

    private List<LandingSite> GenerateLandingSites(int min, int max)
    {
        var sites = new List<LandingSite>();
        int count = _random.Value.Next(min, max + 1);

        for (int i = 0; i < count; i++)
        {
            sites.Add(new LandingSite
            {
                Name = $"Landing Site {i + 1}",
                MaxStationaryArtificialObjects = _random.Value.Next(20, 100),
                MaxMobileArtificialObjects = _random.Value.Next(10, 50),
                CurrentStationaryArtificialObjects = 0,
                CurrentMobileArtificialObjects = 0,
                StaticArtificialObjects = new List<StaticArtificialObject>(),
                MobileArtificialObjects = new List<MobileArtificialObject>()
            });
        }

        return sites;
    }
}