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
            var planetarySystem = GeneratePlanetarySystem(i, planetCount, star.Id, star.Mass, ref previousDistance);
            solarSystem.PlanetarySystems.Add(planetarySystem);
            solarSystem.PlanetarySystemIds.Add(planetarySystem.Id);
        }

        // Phase 3: Generate asteroid belt
        var asteroidBelt = GenerateAsteroidBelt(star.Id, star.Mass);
        foreach (var asteroidSystem in asteroidBelt)
        {
            solarSystem.PlanetarySystems.Add(asteroidSystem);
            solarSystem.PlanetarySystemIds.Add(asteroidSystem.Id);
        }

        // Phase 4: Generate comet cloud
        var comets = GenerateComets(star.Id, star.Mass);
        foreach (var cometSystem in comets)
        {
            solarSystem.PlanetarySystems.Add(cometSystem);
            solarSystem.PlanetarySystemIds.Add(cometSystem.Id);
        }

        int totalBodies = planetCount + asteroidBelt.Count + comets.Count;
        ErrorHandler.LogMessage($"Solar system generated: {planetCount} planets, {asteroidBelt.Count} asteroids, {comets.Count} comets", severity: ErrorUtilities.MessageLevel.Info);
        _eventBus?.Publish(new SolarSystemGeneratedEvent(solarSystem.Id, totalBodies));

        return solarSystem;
    }

    private BaseNaturalSolarObject GenerateCentralStar(Star starSettings)
    {
        float mass = starSettings.Mass > 0 ? starSettings.Mass : 1.0f;
        float radius = 50f;
        float rotationPeriod = GenerateRotationPeriod(mass, 0f, CelestialBodyType.Star);

        var star = new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = starSettings.DisplayName ?? "Sol",
            BodyType = CelestialBodyType.Star,
            PositionX = 0f,
            PositionY = 0f,
            Mass = mass,
            Radius = radius,
            OrbitalPeriod = 0f,
            SemiMajorAxis = 0f,
            Eccentricity = 0f,
            RotationPeriod = rotationPeriod,
            SunlightLevel = 100,
            Orbits = GenerateOrbitalZonesForBody(mass, radius, rotationPeriod, CelestialBodyType.Star, 0f, 0f),
            ResourceDeposits = new List<GameResource>()
        };

        return star;
    }

    private PlanetarySystem GeneratePlanetarySystem(int index, int totalPlanets, Guid starId, float starMass, ref float previousDistance)
    {
        string planetName = index < PlanetNames.Length ? PlanetNames[index] : $"Planet {index + 1}";

        float distance = CalculateOrbitalDistance(index, ref previousDistance);

        var planet = GeneratePlanet(planetName, distance, starId, starMass);

        var system = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = $"{planetName} System",
            CentralMass = planet,
            CentralMassId = planet.Id,
            CelestialBodies = new List<BaseNaturalSolarObject>(),
            CelestialBodyIds = new List<Guid>()
        };

        if ((planet.BodyType == CelestialBodyType.LargePlanet ||
             planet.BodyType == CelestialBodyType.GasGiant ||
             planet.BodyType == CelestialBodyType.IceGiant)
            && _random.Value.NextDouble() < 0.25)
        {
            int moonCount = _random.Value.Next(1, 4);
            for (int m = 0; m < moonCount; m++)
            {
                var moon = GenerateMoon(m, planet.Id, planet.Mass, planet.BodyType, planet.PositionX, planet.PositionY, planet.SemiMajorAxis);
                system.CelestialBodies.Add(moon);
                system.CelestialBodyIds.Add(moon.Id);
            }
        }

        return system;
    }

    private BaseNaturalSolarObject GeneratePlanet(string name, float distance, Guid parentId, float starMass)
    {
        float mass = GenerateMass(distance);
        float radius = CalculateRadius(mass);
        float eccentricity = GenerateEccentricity();
        float orbitalPeriod = CalculateOrbitalPeriod(distance);

        var bodyType = CelestialBodyClassifier.Classify(mass, radius, eccentricity, CelestialBodyType.Star);
        float rotationPeriod = GenerateRotationPeriod(mass, distance, bodyType);

        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);
        float posX = distance * MathF.Cos(angle);
        float posY = distance * MathF.Sin(angle);

        int sunlightLevel = (int)(100 * (1.0f / (1.0f + distance / SolarSystemConstants.BaseOrbitalRadius)));

        // Determine resources based on body type
        List<GameResource> resources = bodyType switch
        {
            CelestialBodyType.GasGiant => new List<GameResource>(), // No resources - can't harvest from gas giants
            CelestialBodyType.IceGiant => GenerateIceGiantResources(distance),
            _ => GenerateResources(bodyType, distance)
        };

        // Determine landing sites based on body type
        List<LandingSite> landingSites = bodyType switch
        {
            CelestialBodyType.GasGiant => new List<LandingSite>(), // No landing on gas giants
            CelestialBodyType.IceGiant => new List<LandingSite>(), // No landing on ice giants
            _ => GenerateLandingSites(1, 3)
        };

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
            RotationPeriod = rotationPeriod,
            ParentBodyId = parentId,
            SunlightLevel = sunlightLevel,
            ResourceDeposits = resources,
            Orbits = GenerateOrbitalZonesForBody(mass, radius, rotationPeriod, bodyType, starMass, distance),
            LandingSites = landingSites
        };

        return planet;
    }

    private BaseNaturalSolarObject GenerateMoon(int index, Guid parentPlanetId, float parentMass, CelestialBodyType parentBodyType, float planetX, float planetY, float parentSemiMajorAxis)
    {
        float moonDistance = 20f + (index * 15f);
        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);

        float mass = (float)(_random.Value.NextDouble() * 0.1 + 0.01);
        float radius = (float)(_random.Value.NextDouble() * 5 + 2);
        float eccentricity = (float)(_random.Value.NextDouble() * 0.05);

        var bodyType = CelestialBodyClassifier.Classify(mass, radius, eccentricity, parentBodyType);
        float rotationPeriod = GenerateRotationPeriod(mass, moonDistance, bodyType);

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
            RotationPeriod = rotationPeriod,
            ParentBodyId = parentPlanetId,
            SunlightLevel = 50,
            ResourceDeposits = GenerateResources(bodyType, moonDistance),
            Orbits = GenerateOrbitalZonesForBody(mass, radius, rotationPeriod, bodyType, parentMass, moonDistance),
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

    /// <summary>
    /// Generates physics-based orbital zones for a celestial body.
    /// Creates LEO, GEO (if applicable), and HEO orbits with calculated altitudes.
    /// </summary>
    private List<Orbit> GenerateOrbitalZonesForBody(float mass, float radius, float rotationPeriod, CelestialBodyType bodyType, float parentMass, float semiMajorAxis)
    {
        var orbits = new List<Orbit>();

        // Skip orbital zones for asteroids and comets
        if (bodyType == CelestialBodyType.Asteroid || bodyType == CelestialBodyType.Comet)
            return orbits;

        // LEO - Low Orbit
        float leoAltitude = OrbitalMechanicsCalculator.CalculateLowOrbitAltitude(radius);
        float leoRadius = radius + leoAltitude;
        orbits.Add(CreateOrbit(
            "Low Orbit",
            Types.OrbitType.LowOrbit,
            leoAltitude,
            leoRadius,
            mass,
            baseCapacity: 10
        ));

        // GEO - Geostationary Orbit (if applicable)
        if (OrbitalMechanicsCalculator.CanHaveGeostationaryOrbit(bodyType))
        {
            float geoAltitude = OrbitalMechanicsCalculator.CalculateGeostationaryAltitude(
                mass, radius, rotationPeriod, parentMass, semiMajorAxis
            );

            if (geoAltitude > 0)
            {
                float geoRadius = radius + geoAltitude;
                orbits.Add(CreateOrbit(
                    "Geostationary Orbit",
                    Types.OrbitType.GeostationaryOrbit,
                    geoAltitude,
                    geoRadius,
                    mass,
                    baseCapacity: 15
                ));
            }
        }

        // HEO - High Orbit
        float heoAltitude = OrbitalMechanicsCalculator.CalculateHighOrbitAltitude(
            radius, mass, parentMass, semiMajorAxis
        );
        float heoRadius = radius + heoAltitude;
        orbits.Add(CreateOrbit(
            "High Orbit",
            Types.OrbitType.HighOrbit,
            heoAltitude,
            heoRadius,
            mass,
            baseCapacity: 20
        ));

        return orbits;
    }

    /// <summary>
    /// Creates an Orbit with calculated orbital mechanics properties.
    /// </summary>
    private Orbit CreateOrbit(string name, Types.OrbitType orbitType, float altitude, float orbitalRadius, float bodyMass, int baseCapacity)
    {
        float orbitalPeriod = OrbitalMechanicsCalculator.CalculateOrbitalPeriod(bodyMass, orbitalRadius);
        float orbitalVelocity = OrbitalMechanicsCalculator.CalculateOrbitalVelocity(bodyMass, orbitalRadius);

        return new Orbit
        {
            Name = name,
            OrbitType = orbitType,
            Altitude = altitude,
            OrbitalRadius = orbitalRadius,
            OrbitalPeriodAtAltitude = orbitalPeriod,
            OrbitalVelocity = orbitalVelocity,
            MaxStationaryArtificialObjects = _random.Value.Next(baseCapacity / 2, baseCapacity),
            MaxMobileArtificialObjects = _random.Value.Next(baseCapacity, baseCapacity * 3),
            CurrentStationaryArtificialObjects = 0,
            CurrentMobileArtificialObjects = 0,
            StaticArtificialObjects = new List<StaticArtificialObject>(),
            MobileArtificialObjects = new List<MobileArtificialObject>()
        };
    }

    /// <summary>
    /// Generates rotation period based on body type and orbital characteristics.
    /// </summary>
    private float GenerateRotationPeriod(float mass, float distance, CelestialBodyType bodyType)
    {
        return bodyType switch
        {
            // Gas giants rotate fast (10-17 hours) - Jupiter/Saturn-like
            CelestialBodyType.GasGiant => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.GasGiantMaxRotationHours - SolarSystemConstants.GasGiantMinRotationHours)
                + SolarSystemConstants.GasGiantMinRotationHours),

            // Ice giants rotate fast but slightly slower (14-17 hours) - Uranus/Neptune-like
            CelestialBodyType.IceGiant => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.IceGiantMaxRotationHours - SolarSystemConstants.IceGiantMinRotationHours)
                + SolarSystemConstants.IceGiantMinRotationHours),

            // Inner planets have chance of tidal locking (Mercury-like)
            CelestialBodyType.Planet when distance < SolarSystemConstants.BaseOrbitalRadius * 2 =>
                _random.Value.NextDouble() < SolarSystemConstants.InnerPlanetTidalLockProbability
                    ? 0f
                    : (float)(_random.Value.NextDouble()
                        * (SolarSystemConstants.TerrestrialMaxRotationHours - SolarSystemConstants.TerrestrialMinRotationHours)
                        + SolarSystemConstants.TerrestrialMinRotationHours),

            // Regular terrestrial planets (10-50 hours)
            CelestialBodyType.Planet or CelestialBodyType.LargePlanet =>
                (float)(_random.Value.NextDouble()
                    * (SolarSystemConstants.TerrestrialMaxRotationHours - SolarSystemConstants.TerrestrialMinRotationHours)
                    + SolarSystemConstants.TerrestrialMinRotationHours),

            // Dwarf planets often have longer rotation periods
            CelestialBodyType.DwarfPlanet => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.DwarfPlanetMaxRotationHours - SolarSystemConstants.DwarfPlanetMinRotationHours)
                + SolarSystemConstants.DwarfPlanetMinRotationHours),

            // Moons often tidally locked
            CelestialBodyType.Moon => _random.Value.NextDouble() < SolarSystemConstants.MoonTidalLockProbability
                ? 0f
                : (float)(_random.Value.NextDouble()
                    * (SolarSystemConstants.MoonMaxRotationHours - SolarSystemConstants.MoonMinRotationHours)
                    + SolarSystemConstants.MoonMinRotationHours),

            // Stars have their own rotation (in days, converted to hours)
            CelestialBodyType.Star => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.StarMaxRotationDays - SolarSystemConstants.StarMinRotationDays)
                + SolarSystemConstants.StarMinRotationDays) * 24f,

            // Small bodies rotate variably
            CelestialBodyType.Asteroid => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.AsteroidMaxRotationHours - SolarSystemConstants.AsteroidMinRotationHours)
                + SolarSystemConstants.AsteroidMinRotationHours),

            CelestialBodyType.Comet => (float)(_random.Value.NextDouble()
                * (SolarSystemConstants.CometMaxRotationHours - SolarSystemConstants.CometMinRotationHours)
                + SolarSystemConstants.CometMinRotationHours),

            // Default for other types
            _ => SolarSystemConstants.DefaultRotationPeriodHours
        };
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

    /// <summary>
    /// Generates ice giant resources (energy-focused, collected from atmosphere).
    /// Ice giants primarily produce energy from hydrogen, helium-3, and other atmospheric gases.
    /// </summary>
    /// <param name="distance">Distance from the star in game units.</param>
    /// <returns>List of resources available on the ice giant.</returns>
    private List<GameResource> GenerateIceGiantResources(float distance)
    {
        var resources = new List<GameResource>();
        int resourceCount = _random.Value.Next(1, 3);

        for (int i = 0; i < resourceCount; i++)
        {
            // Ice giants are primarily energy sources (hydrogen, helium-3, etc.)
            bool isEnergy = _random.Value.NextDouble() < 0.8;

            resources.Add(new GameResource
            {
                Name = isEnergy ? "Energy" : "Minerals",
                ResourceType = isEnergy ? ResourceTypes.Energy : ResourceTypes.Minerals,
                Quantity = _random.Value.Next(200, 1500),
                MaxQuantity = _random.Value.Next(10000, 50000)
            });
        }

        return resources;
    }

    /// <summary>
    /// Generates an asteroid belt around the star.
    /// </summary>
    /// <param name="starId">The ID of the parent star.</param>
    /// <param name="starMass">The mass of the parent star.</param>
    /// <returns>List of planetary systems containing asteroids.</returns>
    private List<PlanetarySystem> GenerateAsteroidBelt(Guid starId, float starMass)
    {
        var asteroidSystems = new List<PlanetarySystem>();
        int asteroidCount = _random.Value.Next(
            SolarSystemConstants.MinAsteroidBeltObjects,
            SolarSystemConstants.MaxAsteroidBeltObjects + 1
        );

        float innerEdge = SolarSystemConstants.BaseOrbitalRadius * SolarSystemConstants.AsteroidBeltInnerMultiplier;
        float outerEdge = SolarSystemConstants.BaseOrbitalRadius * SolarSystemConstants.AsteroidBeltOuterMultiplier;

        for (int i = 0; i < asteroidCount; i++)
        {
            float distance = innerEdge + (float)(_random.Value.NextDouble() * (outerEdge - innerEdge));
            var asteroid = GenerateAsteroid(i, distance, starId, starMass);

            var system = new PlanetarySystem
            {
                Id = Guid.NewGuid(),
                Name = $"Asteroid {i + 1}",
                CentralMass = asteroid,
                CentralMassId = asteroid.Id,
                CelestialBodies = new List<BaseNaturalSolarObject>(),
                CelestialBodyIds = new List<Guid>()
            };

            asteroidSystems.Add(system);
        }

        return asteroidSystems;
    }

    /// <summary>
    /// Generates a single asteroid.
    /// </summary>
    private BaseNaturalSolarObject GenerateAsteroid(int index, float distance, Guid parentId, float parentMass)
    {
        float mass = (float)(_random.Value.NextDouble() * 0.0001 + 0.00001); // Very small mass
        float radius = (float)(_random.Value.NextDouble() * 2 + 0.5);        // Small radius
        float eccentricity = (float)(_random.Value.NextDouble() * 0.3);      // Moderate eccentricity
        float orbitalPeriod = CalculateOrbitalPeriod(distance);
        float rotationPeriod = GenerateRotationPeriod(mass, distance, CelestialBodyType.Asteroid);

        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);
        float posX = distance * MathF.Cos(angle);
        float posY = distance * MathF.Sin(angle);

        int sunlightLevel = (int)(100 * (1.0f / (1.0f + distance / SolarSystemConstants.BaseOrbitalRadius)));

        return new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = $"Asteroid {index + 1}",
            BodyType = CelestialBodyType.Asteroid,
            PositionX = posX,
            PositionY = posY,
            Mass = mass,
            Radius = radius,
            OrbitalPeriod = orbitalPeriod,
            SemiMajorAxis = distance,
            Eccentricity = eccentricity,
            RotationPeriod = rotationPeriod,
            ParentBodyId = parentId,
            SunlightLevel = sunlightLevel,
            ResourceDeposits = GenerateAsteroidResources(),
            Orbits = new List<Orbit>(), // Asteroids don't have orbital zones
            LandingSites = GenerateLandingSites(1, 1) // Single landing site
        };
    }

    /// <summary>
    /// Generates asteroid resources (80% chance of minerals).
    /// Asteroids are primarily composed of rocky and metallic materials.
    /// </summary>
    /// <returns>List of resources available on the asteroid.</returns>
    private List<GameResource> GenerateAsteroidResources()
    {
        var resources = new List<GameResource>();

        // 80% chance to have minerals
        if (_random.Value.NextDouble() < 0.8)
        {
            resources.Add(new GameResource
            {
                Name = "Minerals",
                ResourceType = ResourceTypes.Minerals,
                Quantity = _random.Value.Next(50, 500),
                MaxQuantity = _random.Value.Next(500, 2000)
            });
        }

        return resources;
    }

    /// <summary>
    /// Generates comets in the outer solar system.
    /// </summary>
    /// <param name="starId">The ID of the parent star.</param>
    /// <param name="starMass">The mass of the parent star.</param>
    /// <returns>List of planetary systems containing comets.</returns>
    private List<PlanetarySystem> GenerateComets(Guid starId, float starMass)
    {
        var cometSystems = new List<PlanetarySystem>();
        int cometCount = _random.Value.Next(
            SolarSystemConstants.MinComets,
            SolarSystemConstants.MaxComets + 1
        );

        for (int i = 0; i < cometCount; i++)
        {
            var comet = GenerateComet(i, starId, starMass);

            var system = new PlanetarySystem
            {
                Id = Guid.NewGuid(),
                Name = $"Comet {i + 1}",
                CentralMass = comet,
                CentralMassId = comet.Id,
                CelestialBodies = new List<BaseNaturalSolarObject>(),
                CelestialBodyIds = new List<Guid>()
            };

            cometSystems.Add(system);
        }

        return cometSystems;
    }

    /// <summary>
    /// Generates a single comet with high eccentricity orbit.
    /// </summary>
    private BaseNaturalSolarObject GenerateComet(int index, Guid parentId, float parentMass)
    {
        float innerEdge = SolarSystemConstants.BaseOrbitalRadius * SolarSystemConstants.CometZoneMinMultiplier;
        float outerEdge = SolarSystemConstants.BaseOrbitalRadius * SolarSystemConstants.CometZoneMaxMultiplier;
        float distance = innerEdge + (float)(_random.Value.NextDouble() * (outerEdge - innerEdge));

        float mass = (float)(_random.Value.NextDouble() * 0.00001 + 0.000001); // Very small mass
        float radius = (float)(_random.Value.NextDouble() * 1.5 + 0.3);        // Small radius

        // Comets have highly elliptical orbits
        float eccentricity = SolarSystemConstants.CometMinEccentricity +
            (float)(_random.Value.NextDouble() *
            (SolarSystemConstants.CometMaxEccentricity - SolarSystemConstants.CometMinEccentricity));

        float orbitalPeriod = CalculateOrbitalPeriod(distance);
        float rotationPeriod = GenerateRotationPeriod(mass, distance, CelestialBodyType.Comet);

        float angle = (float)(_random.Value.NextDouble() * 2 * Math.PI);
        float posX = distance * MathF.Cos(angle);
        float posY = distance * MathF.Sin(angle);

        int sunlightLevel = (int)(100 * (1.0f / (1.0f + distance / SolarSystemConstants.BaseOrbitalRadius)));

        return new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = $"Comet {index + 1}",
            BodyType = CelestialBodyType.Comet,
            PositionX = posX,
            PositionY = posY,
            Mass = mass,
            Radius = radius,
            OrbitalPeriod = orbitalPeriod,
            SemiMajorAxis = distance,
            Eccentricity = eccentricity,
            RotationPeriod = rotationPeriod,
            ParentBodyId = parentId,
            SunlightLevel = sunlightLevel,
            ResourceDeposits = GenerateCometResources(),
            Orbits = new List<Orbit>(), // Comets don't have orbital zones
            LandingSites = null // No landing on comets
        };
    }

    /// <summary>
    /// Generates comet resources (60% energy, 30% minerals).
    /// Comets contain volatile ices that can be harvested for energy,
    /// along with some mineral content in their rocky cores.
    /// </summary>
    /// <returns>List of resources available on the comet.</returns>
    private List<GameResource> GenerateCometResources()
    {
        var resources = new List<GameResource>();

        // 60% chance for energy (volatile ices, etc.)
        if (_random.Value.NextDouble() < 0.6)
        {
            resources.Add(new GameResource
            {
                Name = "Energy",
                ResourceType = ResourceTypes.Energy,
                Quantity = _random.Value.Next(30, 200),
                MaxQuantity = _random.Value.Next(200, 1000)
            });
        }

        // 30% chance for minerals
        if (_random.Value.NextDouble() < 0.3)
        {
            resources.Add(new GameResource
            {
                Name = "Minerals",
                ResourceType = ResourceTypes.Minerals,
                Quantity = _random.Value.Next(20, 100),
                MaxQuantity = _random.Value.Next(100, 500)
            });
        }

        return resources;
    }
}