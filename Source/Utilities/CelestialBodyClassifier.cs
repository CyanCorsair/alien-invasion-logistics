using AlienInvasionLogistics.Source.Database.Models;

namespace AlienInvasionLogistics.Source.Utilities;

/// <summary>
/// Classifies celestial bodies based on IAU definitions.
/// Mass units are relative to Earth mass (1.0 = Earth).
/// </summary>
public static class CelestialBodyClassifier
{
    // Mass thresholds (Earth masses)
    private const float StarMinMass = 4000f;           // ~13 Jupiter masses (brown dwarf minimum)
    private const float GasGiantMinMass = 50f;         // Hydrogen/helium composition (Jupiter/Saturn-like)
    private const float IceGiantMinMass = 10f;         // Water/ammonia/methane composition (Uranus/Neptune-like)
    private const float LargePlanetMinMass = 2f;       // Super-Earth threshold
    private const float PlanetMinMass = 0.1f;          // Terrestrial planet minimum
    private const float DwarfPlanetMinMass = 0.001f;   // Hydrostatic equilibrium threshold

    // Radius threshold for hydrostatic equilibrium (game units, ~400km equivalent)
    private const float HydrostaticEquilibriumRadius = 5f;

    // Eccentricity threshold distinguishing comets from asteroids
    private const float CometEccentricityThreshold = 0.5f;

    /// <summary>
    /// Classifies a celestial body based on its physical and orbital properties.
    /// </summary>
    /// <param name="mass">Mass in Earth masses</param>
    /// <param name="radius">Radius in game units</param>
    /// <param name="eccentricity">Orbital eccentricity (0-1)</param>
    /// <param name="parentBodyType">Type of the parent body, if any</param>
    public static CelestialBodyType Classify(
        float mass,
        float radius,
        float eccentricity,
        CelestialBodyType? parentBodyType = null)
    {
        // Stars: massive enough for fusion
        if (mass >= StarMinMass)
            return CelestialBodyType.Star;

        // Moons: any body orbiting a planet (not a star)
        if (parentBodyType is CelestialBodyType.Planet
            or CelestialBodyType.LargePlanet
            or CelestialBodyType.GasGiant
            or CelestialBodyType.IceGiant
            or CelestialBodyType.DwarfPlanet)
        {
            return CelestialBodyType.Moon;
        }

        // Gas giants: massive hydrogen/helium bodies (>= 50 Earth masses)
        if (mass >= GasGiantMinMass)
            return CelestialBodyType.GasGiant;

        // Ice giants: intermediate mass with water/ammonia/methane (10-50 Earth masses)
        if (mass >= IceGiantMinMass)
            return CelestialBodyType.IceGiant;

        // Large planets: super-Earths, mini-Neptunes
        if (mass >= LargePlanetMinMass)
            return CelestialBodyType.LargePlanet;

        // Planets: terrestrial worlds
        if (mass >= PlanetMinMass)
            return CelestialBodyType.Planet;

        // Dwarf planets: in hydrostatic equilibrium but small
        if (mass >= DwarfPlanetMinMass && radius >= HydrostaticEquilibriumRadius)
            return CelestialBodyType.DwarfPlanet;

        // Small bodies: distinguish comet vs asteroid by eccentricity
        if (eccentricity >= CometEccentricityThreshold)
            return CelestialBodyType.Comet;

        // Minor planets / asteroids
        if (mass >= DwarfPlanetMinMass)
            return CelestialBodyType.MinorPlanet;

        return CelestialBodyType.Asteroid;
    }

    /// <summary>
    /// Classifies directly from a BaseNaturalSolarObject.
    /// </summary>
    public static CelestialBodyType Classify(
        BaseNaturalSolarObject body,
        CelestialBodyType? parentBodyType = null)
    {
        return Classify(body.Mass, body.Radius, body.Eccentricity, parentBodyType);
    }
}
