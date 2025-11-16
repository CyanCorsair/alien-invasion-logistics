using Godot;
using System;
using System.Collections.Generic;
using Core.Database.Models;
using Core.GameObjects;

namespace Core.Services
{
    /// <summary>
    /// Generates solar systems based on game settings
    /// </summary>
    public class SolarSystemGenerator
    {
        private static readonly string[] STAR_TYPES =
        {
            "Red Dwarf",
            "Yellow Star",
            "Blue Giant",
            "White Dwarf"
        };
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

        private readonly Random _random;

        public SolarSystemGenerator(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>
        /// Generate solar system state from game settings
        /// </summary>
        public SolarSystemState GenerateSolarSystemState(GameSettingsModel settings)
        {
            var solarSystemState = new SolarSystemState { Id = Guid.NewGuid() };

            // Generate the sun
            var sunState = GenerateSunState(settings.StarType);
            solarSystemState.SystemSun = sunState;
            solarSystemState.SystemSunId = sunState.Id;

            // Generate planets based on NumberOfPlanets
            var planetarySystem = GeneratePlanetarySystem(
                settings.NumberOfPlanets,
                sunState.DisplayName,
                solarSystemState.Id
            );
            solarSystemState.PlanetarySystems.Add(planetarySystem);

            GD.Print(
                $"Generated solar system with {settings.NumberOfPlanets} planets around {sunState.DisplayName}"
            );

            return solarSystemState;
        }

        /// <summary>
        /// Generate sun state based on star type
        /// </summary>
        private SunStateModel GenerateSunState(int starType)
        {
            // Clamp star type to valid range
            starType = Mathf.Clamp(starType, 0, STAR_TYPES.Length - 1);

            var sunState = new SunStateModel
            {
                Id = Guid.NewGuid(),
                DisplayName = STAR_TYPES[starType],
                SystemName = "Primary System",
                PositionX = 0f, // Sun at center
                PositionY = 0f,
                VelocityX = 0f,
                VelocityY = 0f,
                Mass = GetStarMass(starType)
            };

            return sunState;
        }

        /// <summary>
        /// Generate a planetary system with multiple planets
        /// </summary>
        private PlanetarySystemState GeneratePlanetarySystem(
            int numberOfPlanets,
            string systemName,
            Guid solarSystemId
        )
        {
            var planetarySystem = new PlanetarySystemState
            {
                Id = Guid.NewGuid(),
                Name = $"{systemName} Planetary System",
                SolarSystemStateId = solarSystemId,
                Planets = new List<PlanetStateModel>()
            };

            // Generate planets at increasing orbital radii
            float baseOrbitalRadius = 150f; // Starting orbital distance
            float orbitalRadiusIncrement = 100f; // Distance between orbits

            for (int i = 0; i < numberOfPlanets; i++)
            {
                float orbitalRadius = baseOrbitalRadius + (i * orbitalRadiusIncrement);

                // Add some randomness to orbital radius
                orbitalRadius += (float)(_random.NextDouble() * 50 - 25);

                var planetState = GeneratePlanetState(
                    i,
                    systemName,
                    orbitalRadius,
                    planetarySystem.Id
                );

                planetarySystem.Planets.Add(planetState);
            }

            return planetarySystem;
        }

        /// <summary>
        /// Generate a single planet state
        /// </summary>
        private PlanetStateModel GeneratePlanetState(
            int planetIndex,
            string systemName,
            float orbitalRadius,
            Guid planetarySystemId
        )
        {
            // Get planet name (or generate if we run out of names)
            string planetName =
                planetIndex < PLANET_NAMES.Length
                    ? PLANET_NAMES[planetIndex]
                    : $"Planet {planetIndex + 1}";

            // Calculate initial position on orbit (random angle)
            float initialAngle = (float)(_random.NextDouble() * Math.PI * 2);
            float posX = orbitalRadius * Mathf.Cos(initialAngle);
            float posY = orbitalRadius * Mathf.Sin(initialAngle);

            // Calculate orbital velocity (Kepler's laws simplified)
            // Velocity decreases with orbital radius
            float orbitalSpeed = 50f / Mathf.Sqrt(orbitalRadius);

            // Velocity is perpendicular to radius (tangent to orbit)
            float velX = -orbitalSpeed * Mathf.Sin(initialAngle);
            float velY = orbitalSpeed * Mathf.Cos(initialAngle);

            var planetState = new PlanetStateModel
            {
                Id = Guid.NewGuid(),
                DisplayName = planetName,
                SystemName = systemName,
                PositionX = posX,
                PositionY = posY,
                VelocityX = velX,
                VelocityY = velY,
                Mass = GetRandomPlanetMass(),
                PlanetarySystemId = planetarySystemId
            };

            return planetState;
        }

        /// <summary>
        /// Get star mass based on star type
        /// </summary>
        private float GetStarMass(int starType)
        {
            return starType switch
            {
                0 => 0.5e30f, // Red Dwarf (smaller)
                1 => 1.989e30f, // Yellow Star (Sun-like)
                2 => 3.5e30f, // Blue Giant (larger)
                3 => 0.6e30f, // White Dwarf (dense but small)
                _ => 1.989e30f // Default to Sun-like
            };
        }

        /// <summary>
        /// Generate random planet mass
        /// </summary>
        private float GetRandomPlanetMass()
        {
            // Random mass between Earth-like (5.972e24) and Jupiter-like (1.898e27)
            double logMin = Math.Log(5.972e24);
            double logMax = Math.Log(1.898e27);
            double randomLog = logMin + (_random.NextDouble() * (logMax - logMin));

            return (float)Math.Exp(randomLog);
        }

        /// <summary>
        /// Instantiate Sun Node2D from state
        /// </summary>
        public Sun InstantiateSunFromState(SunStateModel sunState)
        {
            var sun = new Sun
            {
                DisplayName = sunState.DisplayName,
                SystemName = sunState.SystemName,
                Position = new Vector2(sunState.PositionX, sunState.PositionY),
                Velocity2D = new Vector2(sunState.VelocityX, sunState.VelocityY),
                Mass = sunState.Mass
            };

            return sun;
        }

        /// <summary>
        /// Instantiate Planet Node2D from state
        /// </summary>
        public Planet InstantiatePlanetFromState(PlanetStateModel planetState, Vector2 orbitCenter)
        {
            var planet = new Planet
            {
                DisplayName = planetState.DisplayName,
                SystemName = planetState.SystemName,
                Position = new Vector2(planetState.PositionX, planetState.PositionY),
                Velocity2D = new Vector2(planetState.VelocityX, planetState.VelocityY),
                Mass = planetState.Mass
            };

            // Calculate orbital parameters from state
            Vector2 planetPos = new Vector2(planetState.PositionX, planetState.PositionY);
            Vector2 relativePos = planetPos - orbitCenter;

            planet.OrbitalRadius = relativePos.Length();
            planet.SetOrbitCenter(orbitCenter);

            // Calculate orbital speed from velocity
            Vector2 velocity = new Vector2(planetState.VelocityX, planetState.VelocityY);
            planet.OrbitalSpeed = velocity.Length() / planet.OrbitalRadius;

            return planet;
        }

        /// <summary>
        /// Save Node2D state back to state model
        /// </summary>
        public SunStateModel SaveSunState(Sun sun, SunStateModel existingState = null)
        {
            var state = existingState ?? new SunStateModel();

            state.DisplayName = sun.DisplayName;
            state.SystemName = sun.SystemName;
            state.PositionX = sun.Position.X;
            state.PositionY = sun.Position.Y;
            state.VelocityX = sun.Velocity2D.X;
            state.VelocityY = sun.Velocity2D.Y;
            state.Mass = sun.Mass;

            return state;
        }

        /// <summary>
        /// Save Planet Node2D state back to state model
        /// </summary>
        public PlanetStateModel SavePlanetState(
            Planet planet,
            PlanetStateModel existingState = null
        )
        {
            var state = existingState ?? new PlanetStateModel();

            state.DisplayName = planet.DisplayName;
            state.SystemName = planet.SystemName;
            state.PositionX = planet.Position.X;
            state.PositionY = planet.Position.Y;
            state.VelocityX = planet.Velocity2D.X;
            state.VelocityY = planet.Velocity2D.Y;
            state.Mass = planet.Mass;

            return state;
        }
    }
}
