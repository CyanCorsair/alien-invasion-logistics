using System;
using AlienInvasionLogistics.Source.Constants;
using AlienInvasionLogistics.Source.Database.Models;

namespace AlienInvasionLogistics.Source.Utilities;

/// <summary>
///     Static utility class for orbital mechanics calculations
/// </summary>
public static class OrbitalMechanicsCalculator
{
    /// <summary>
    ///     Calculates the Low Orbit (LEO) altitude above the body's surface
    /// </summary>
    /// <param name="bodyRadius">Radius of the celestial body</param>
    /// <returns>Altitude above surface for low orbit</returns>
    public static float CalculateLowOrbitAltitude(float bodyRadius)
    {
        return bodyRadius * (SolarSystemConstants.LowOrbitMultiplier - 1f);
    }

    /// <summary>
    ///     Calculates the Geostationary Orbit (GEO) altitude where orbital period matches rotation period
    /// </summary>
    /// <param name="mass">Mass of the celestial body</param>
    /// <param name="radius">Radius of the celestial body</param>
    /// <param name="rotationPeriodHours">Rotation period in hours</param>
    /// <param name="parentMass">Mass of the parent body</param>
    /// <param name="semiMajorAxis">Semi-major axis of orbit around parent</param>
    /// <returns>Altitude above surface for geostationary orbit, or 0 if not possible</returns>
    public static float CalculateGeostationaryAltitude(float mass, float radius, float rotationPeriodHours, float parentMass, float semiMajorAxis)
    {
        // Tidally locked bodies (rotation period = 0 or very long) cannot have geostationary orbits
        if (rotationPeriodHours <= 0)
            return 0f;

        // Convert rotation period to seconds
        float rotationPeriodSeconds = rotationPeriodHours * 3600f;

        // Using Kepler's third law: r = (G * M * T^2 / (4 * pi^2))^(1/3)
        // Where G is gravitational constant, M is body mass, T is rotation period
        float gm = SolarSystemConstants.GravitationalConstant * mass * SolarSystemConstants.EarthMassKg;
        float orbitalRadius = MathF.Pow(
            (gm * rotationPeriodSeconds * rotationPeriodSeconds) / (4f * MathF.PI * MathF.PI),
            1f / 3f
        );

        // Convert to game units (assuming radius is in game units)
        // Scale factor: normalize relative to body radius
        float geoAltitude = (orbitalRadius / 1000f) - radius; // Convert m to km-scale then subtract radius

        // If GEO altitude is too low (inside the body) or negative, it's not valid
        if (geoAltitude <= 0)
            return 0f;

        // Check if GEO is beyond the Hill sphere
        float hillRadius = CalculateHillSphereRadius(mass, parentMass, semiMajorAxis);
        if (geoAltitude + radius > hillRadius * SolarSystemConstants.HillSpherePracticalLimit)
            return 0f;

        return geoAltitude;
    }

    /// <summary>
    ///     Calculates the High Orbit (HEO) altitude based on the Hill sphere
    /// </summary>
    /// <param name="radius">Radius of the celestial body</param>
    /// <param name="mass">Mass of the celestial body</param>
    /// <param name="parentMass">Mass of the parent body</param>
    /// <param name="semiMajorAxis">Semi-major axis of orbit around parent</param>
    /// <returns>Altitude above surface for high orbit</returns>
    public static float CalculateHighOrbitAltitude(float radius, float mass, float parentMass, float semiMajorAxis)
    {
        float hillRadius = CalculateHillSphereRadius(mass, parentMass, semiMajorAxis);

        // Use practical limit of Hill sphere for stable high orbit
        float heoRadius = hillRadius * SolarSystemConstants.HillSpherePracticalLimit;

        // Ensure HEO is at least HighOrbitMultiplier times the body radius
        float minHeoRadius = radius * SolarSystemConstants.HighOrbitMultiplier;
        heoRadius = MathF.Max(heoRadius, minHeoRadius);

        return heoRadius - radius;
    }

    /// <summary>
    ///     Calculates the Hill sphere radius for a body orbiting a parent
    /// </summary>
    /// <param name="bodyMass">Mass of the orbiting body</param>
    /// <param name="parentMass">Mass of the parent body</param>
    /// <param name="semiMajorAxis">Semi-major axis of the orbit</param>
    /// <returns>Hill sphere radius</returns>
    public static float CalculateHillSphereRadius(float bodyMass, float parentMass, float semiMajorAxis)
    {
        // Hill sphere: r_H = a * (m / (3 * M))^(1/3)
        // Where a is semi-major axis, m is body mass, M is parent mass
        if (bodyMass <= 0 || parentMass <= 0 || semiMajorAxis <= 0)
            return semiMajorAxis > 0 ? semiMajorAxis * 0.5f : 100f; // Fallback for stars or invalid cases

        return semiMajorAxis * MathF.Pow(bodyMass / (3f * parentMass), 1f / 3f);
    }

    /// <summary>
    ///     Calculates the orbital period for an object at a given orbital radius
    /// </summary>
    /// <param name="bodyMass">Mass of the central body</param>
    /// <param name="orbitalRadius">Radius of the orbit from body center</param>
    /// <returns>Orbital period in hours</returns>
    public static float CalculateOrbitalPeriod(float bodyMass, float orbitalRadius)
    {
        if (bodyMass <= 0 || orbitalRadius <= 0)
            return 0f;

        // T = 2 * pi * sqrt(r^3 / (G * M))
        float gm = SolarSystemConstants.GravitationalConstant * bodyMass * SolarSystemConstants.EarthMassKg;
        float orbitalRadiusMeters = orbitalRadius * 1000f; // Convert to meters

        float periodSeconds = 2f * MathF.PI * MathF.Sqrt(
            (orbitalRadiusMeters * orbitalRadiusMeters * orbitalRadiusMeters) / gm
        );

        return periodSeconds / 3600f; // Convert to hours
    }

    /// <summary>
    ///     Calculates the orbital velocity at a given orbital radius
    /// </summary>
    /// <param name="bodyMass">Mass of the central body</param>
    /// <param name="orbitalRadius">Radius of the orbit from body center</param>
    /// <returns>Orbital velocity in km/s</returns>
    public static float CalculateOrbitalVelocity(float bodyMass, float orbitalRadius)
    {
        if (bodyMass <= 0 || orbitalRadius <= 0)
            return 0f;

        // v = sqrt(G * M / r)
        float gm = SolarSystemConstants.GravitationalConstant * bodyMass * SolarSystemConstants.EarthMassKg;
        float orbitalRadiusMeters = orbitalRadius * 1000f;

        return MathF.Sqrt(gm / orbitalRadiusMeters) / 1000f; // Convert to km/s
    }

    /// <summary>
    ///     Determines if a celestial body type can have a geostationary orbit
    /// </summary>
    /// <param name="bodyType">Type of celestial body</param>
    /// <returns>True if the body can support geostationary orbits</returns>
    public static bool CanHaveGeostationaryOrbit(CelestialBodyType bodyType)
    {
        return bodyType switch
        {
            CelestialBodyType.Star => false,
            CelestialBodyType.Comet => false,
            CelestialBodyType.Asteroid => false,
            CelestialBodyType.MinorPlanet => false,
            // GasGiant and IceGiant can have geostationary orbits
            CelestialBodyType.GasGiant => true,
            CelestialBodyType.IceGiant => true,
            _ => true
        };
    }
}
