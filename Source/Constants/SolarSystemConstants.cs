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
}