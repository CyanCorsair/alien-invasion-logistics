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
    private static readonly string[] PlanetNames =
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

        int planetCount = Math.Min(settings.CelestialBodyCount, SolarSystemConstants.MaxPlanets);
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

    private BaseNaturalSolarObject GenerateCentralStar(Star starSettings)
    {
        var star = new BaseNaturalSolarObject
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
        string planetName = index < PlanetNames.Length ? PlanetNames[index] : $"Planet {index + 1}";

        float distance = CalculateOrbitalDistance(index, ref previousDistance);

        var planet = GeneratePlanet(planetName, distance, starId);

        var system = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = $"{planetName} System",
            CentralMass = planet,
            CentralMassId = planet.Id,
            CelestialBodies = new List<BaseNaturalSolarObject>(),
            CelestialBodyIds = new List<Guid>()
        };

        if ((planet.BodyType == CelestialBodyType.LargePlanet || planet.BodyType == CelestialBodyType.GiantPlanet)
            && _random.Value.NextDouble() < 0.25)
        {
            int moonCount = _random.Value.Next(1, 4);
            for (int m = 0; m < moonCount; m++)
            {
                var moon = GenerateMoon(m, planet.Id, planet.BodyType, planet.PositionX, planet.PositionY);
                system.CelestialBodies.Add(moon);
                system.CelestialBodyIds.Add(moon.Id);
            }
        }

        return system;
    }

    private BaseNaturalSolarObject GeneratePlanet(string name, float distance, Guid parentId)
    {
        float mass = GenerateMass(distance);
        float radius = CalculateRadius(mass);
        float eccentricity = GenerateEccentricity();
        float orbitalPeriod = CalculateOrbitalPeriod(distance);

        var bodyType = CelestialBodyClassifier.Classify(mass, radius, eccentricity, CelestialBodyType.Star);

        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);
        float posX = distance * MathF.Cos(angle);
        float posY = distance * MathF.Sin(angle);

        int sunlightLevel = (int)(100 * (1.0f / (1.0f + distance / SolarSystemConstants.BaseOrbitalRadius)));

        var planet = new BaseNaturalSolarObject
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

    private BaseNaturalSolarObject GenerateMoon(int index, Guid parentPlanetId, CelestialBodyType parentBodyType, float planetX, float planetY)
    {
        float moonDistance = 20f + (index * 15f);
        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);

        float mass = (float)(_random.Value.NextDouble() * 0.1 + 0.01);
        float radius = (float)(_random.Value.NextDouble() * 5 + 2);
        float eccentricity = (float)(_random.Value.NextDouble() * 0.05);

        var bodyType = CelestialBodyClassifier.Classify(mass, radius, eccentricity, parentBodyType);

        var moon = new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = $"Moon {index + 1}",
            BodyType = bodyType,
            PositionX = planetX + moonDistance * MathF.Cos(angle),
            PositionY = planetY + moonDistance * MathF.Sin(angle),
            Mass = mass,
            Radius = radius,
            OrbitalPeriod = CalculateOrbitalPeriod(moonDistance) * 0.1f,
            SemiMajorAxis = moonDistance,
            Eccentricity = eccentricity,
            ParentBodyId = parentPlanetId,
            SunlightLevel = 50,
            ResourceDeposits = GenerateResources(bodyType, moonDistance),
            Orbits = GenerateOrbitalZones(1, 2),
            LandingSites = GenerateLandingSites(1, 2)
        };

        return moon;
    }

    private float CalculateOrbitalDistance(int index, ref float previousDistance)
    {
        float baseDistance = SolarSystemConstants.BaseOrbitalRadius * MathF.Pow(1.7f, index);

        float variance = (float)(_random.Value.NextDouble() - 0.5) * SolarSystemConstants.OrbitalRadiusVariance;
        float distance = baseDistance + variance;

        if (distance < previousDistance + 50f)
            distance = previousDistance + 50f;

        previousDistance = distance;
        return distance;
    }

    /// <summary>
    /// Generates mass based on orbital distance, following the pattern of our solar system:
    /// inner planets tend to be smaller/rocky, mid-range has gas giants, outer range has ice giants and dwarf planets.
    /// </summary>
    private float GenerateMass(float distance)
    {
        float normalizedDistance = distance / SolarSystemConstants.BaseOrbitalRadius;
        double roll = _random.Value.NextDouble();

        // Inner zone (< 2x base): rocky planets
        if (normalizedDistance < 2f)
        {
            return roll < 0.7f
                ? (float)(_random.Value.NextDouble() * 1.5 + 0.1)   // Terrestrial (0.1 - 1.6 Earth masses)
                : (float)(_random.Value.NextDouble() * 5 + 2);      // Super-Earth (2 - 7 Earth masses)
        }

        // Middle zone (2-6x base): gas/ice giants
        if (normalizedDistance < 6f)
        {
            return roll < 0.6f
                ? (float)(_random.Value.NextDouble() * 200 + 50)    // Gas giant (50 - 250 Earth masses)
                : (float)(_random.Value.NextDouble() * 30 + 10);    // Ice giant (10 - 40 Earth masses)
        }

        // Outer zone (> 6x base): ice giants, dwarf planets, small bodies
        if (roll < 0.3f)
            return (float)(_random.Value.NextDouble() * 20 + 10);   // Ice giant
        if (roll < 0.7f)
            return (float)(_random.Value.NextDouble() * 0.1 + 0.001); // Dwarf planet
        return (float)(_random.Value.NextDouble() * 0.001);          // Minor planet/asteroid
    }

    /// <summary>
    /// Calculates radius from mass. Gas giants have lower density, so they get a larger radius multiplier.
    /// </summary>
    private float CalculateRadius(float mass)
    {
        // Gas giants (> 50 Earth masses) have lower density
        float multiplier = mass >= 50f ? 3.0f : 1.0f;
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
        if (!(SolarSystemConstants.BaseOrbitalRadius <= 0))
            return MathF.Sqrt(MathF.Pow(semiMajorAxis / SolarSystemConstants.BaseOrbitalRadius, 3))
                   * SolarSystemConstants.OrbitalSpeedBase;
    }

    private List<GameResource> GenerateResources(CelestialBodyType bodyType, float distance)
    {
        var resources = new List<GameResource>();
        int resourceCount = _random.Value.Next(0, 4);

        for (int i = 0; i < resourceCount; i++)
        {
            bool isMinerals = distance < SolarSystemConstants.BaseOrbitalRadius * 3
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