namespace AlienInvasionLogistics.Source.Constants;

/// <summary>
///     Constants for solar system generation and orbital mechanics
/// </summary>
public static class SolarSystemConstants
{
    /// <summary>
    ///     Astronomical Unit in kilometers (distance from Earth to Sun)
    /// </summary>
    public const float AuInKm = 149597870.7f;

    /// <summary>
    ///     Speed of light in kilometers per second
    /// </summary>
    public const float LightSpeedKmS = 299792.458f;

    /// <summary>
    ///     Gravitational constant in m³ kg⁻¹ s⁻²
    /// </summary>
    public const float GravitationalConstant = 6.67430e-11f;

    /// <summary>
    ///     Maximum number of planets allowed in a solar system
    /// </summary>
    public const int MaxPlanets = 15;

    /// <summary>
    ///     Minimum orbital radius in Astronomical Units
    /// </summary>
    public const float MinOrbitalRadiusAu = 0.1f;

    /// <summary>
    ///     Maximum orbital radius in Astronomical Units
    /// </summary>
    public const float MaxOrbitalRadiusAu = 50f;

    /// <summary>
    ///     Base orbital radius for the first planet in game units
    /// </summary>
    public const float BaseOrbitalRadius = 150f;

    /// <summary>
    ///     Distance increment between planetary orbits in game units
    /// </summary>
    public const float OrbitalRadiusIncrement = 100f;

    /// <summary>
    ///     Maximum random variance applied to orbital radius
    /// </summary>
    public const float OrbitalRadiusVariance = 50f;

    /// <summary>
    ///     Base value for orbital speed calculations
    /// </summary>
    public const float OrbitalSpeedBase = 50f;

    /// <summary>
    ///     Minimum orbital radius threshold for division-by-zero prevention
    /// </summary>
    public const float MinOrbitalRadius = 1f;

    /// <summary>
    ///     Default orbital speed when radius is below minimum threshold
    /// </summary>
    public const float DefaultOrbitalSpeed = 1f;

    // Orbital altitude calculation constants

    /// <summary>
    ///     Multiplier for Low Orbit altitude (LEO = radius * multiplier - radius)
    /// </summary>
    public const float LowOrbitMultiplier = 1.2f;

    /// <summary>
    ///     Multiplier for High Orbit altitude relative to body radius
    /// </summary>
    public const float HighOrbitMultiplier = 10f;

    /// <summary>
    ///     Practical limit for Hill sphere usage (0.5 = 50% of Hill sphere radius)
    /// </summary>
    public const float HillSpherePracticalLimit = 0.5f;

    /// <summary>
    ///     Earth mass in kilograms for reference calculations
    /// </summary>
    public const float EarthMassKg = 5.972e24f;

    /// <summary>
    ///     Default rotation period in hours (Earth-like)
    /// </summary>
    public const float DefaultRotationPeriodHours = 24f;

    // Asteroid belt generation constants

    /// <summary>
    ///     Inner edge of asteroid belt as multiplier of BaseOrbitalRadius
    /// </summary>
    public const float AsteroidBeltInnerMultiplier = 2.5f;

    /// <summary>
    ///     Outer edge of asteroid belt as multiplier of BaseOrbitalRadius
    /// </summary>
    public const float AsteroidBeltOuterMultiplier = 3.5f;

    /// <summary>
    ///     Minimum number of asteroids in a belt
    /// </summary>
    public const int MinAsteroidBeltObjects = 5;

    /// <summary>
    ///     Maximum number of asteroids in a belt
    /// </summary>
    public const int MaxAsteroidBeltObjects = 15;

    // Comet generation constants

    /// <summary>
    ///     Inner edge of comet zone as multiplier of BaseOrbitalRadius
    /// </summary>
    public const float CometZoneMinMultiplier = 8f;

    /// <summary>
    ///     Outer edge of comet zone as multiplier of BaseOrbitalRadius
    /// </summary>
    public const float CometZoneMaxMultiplier = 20f;

    /// <summary>
    ///     Minimum number of comets in a system
    /// </summary>
    public const int MinComets = 2;

    /// <summary>
    ///     Maximum number of comets in a system
    /// </summary>
    public const int MaxComets = 8;

    /// <summary>
    ///     Minimum eccentricity for comet orbits
    /// </summary>
    public const float CometMinEccentricity = 0.5f;

    /// <summary>
    ///     Maximum eccentricity for comet orbits
    /// </summary>
    public const float CometMaxEccentricity = 0.95f;

    // Rotation period constants (in hours)

    /// <summary>
    ///     Minimum rotation period for gas giants (Jupiter/Saturn-like)
    /// </summary>
    public const float GasGiantMinRotationHours = 10f;

    /// <summary>
    ///     Maximum rotation period for gas giants (Jupiter/Saturn-like)
    /// </summary>
    public const float GasGiantMaxRotationHours = 17f;

    /// <summary>
    ///     Minimum rotation period for ice giants (Uranus/Neptune-like)
    /// </summary>
    public const float IceGiantMinRotationHours = 14f;

    /// <summary>
    ///     Maximum rotation period for ice giants (Uranus/Neptune-like)
    /// </summary>
    public const float IceGiantMaxRotationHours = 17f;

    /// <summary>
    ///     Minimum rotation period for terrestrial planets
    /// </summary>
    public const float TerrestrialMinRotationHours = 10f;

    /// <summary>
    ///     Maximum rotation period for terrestrial planets
    /// </summary>
    public const float TerrestrialMaxRotationHours = 50f;

    /// <summary>
    ///     Minimum rotation period for dwarf planets
    /// </summary>
    public const float DwarfPlanetMinRotationHours = 10f;

    /// <summary>
    ///     Maximum rotation period for dwarf planets
    /// </summary>
    public const float DwarfPlanetMaxRotationHours = 110f;

    /// <summary>
    ///     Minimum rotation period for moons (non-tidally locked)
    /// </summary>
    public const float MoonMinRotationHours = 10f;

    /// <summary>
    ///     Maximum rotation period for moons (non-tidally locked)
    /// </summary>
    public const float MoonMaxRotationHours = 60f;

    /// <summary>
    ///     Minimum rotation period for stars (in days, converted to hours)
    /// </summary>
    public const float StarMinRotationDays = 10f;

    /// <summary>
    ///     Maximum rotation period for stars (in days, converted to hours)
    /// </summary>
    public const float StarMaxRotationDays = 30f;

    /// <summary>
    ///     Minimum rotation period for asteroids
    /// </summary>
    public const float AsteroidMinRotationHours = 2f;

    /// <summary>
    ///     Maximum rotation period for asteroids
    /// </summary>
    public const float AsteroidMaxRotationHours = 22f;

    /// <summary>
    ///     Minimum rotation period for comets
    /// </summary>
    public const float CometMinRotationHours = 5f;

    /// <summary>
    ///     Maximum rotation period for comets
    /// </summary>
    public const float CometMaxRotationHours = 55f;

    /// <summary>
    ///     Probability of tidal locking for inner planets
    /// </summary>
    public const float InnerPlanetTidalLockProbability = 0.3f;

    /// <summary>
    ///     Probability of tidal locking for moons
    /// </summary>
    public const float MoonTidalLockProbability = 0.7f;
}