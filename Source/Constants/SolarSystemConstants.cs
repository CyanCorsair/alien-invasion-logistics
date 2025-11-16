namespace AlienInvasionLogistics.Constants;

/// <summary>
/// Constants for solar system generation and orbital mechanics
/// </summary>
public static class SolarSystemConstants
{
    /// <summary>
    /// Astronomical Unit in kilometers (distance from Earth to Sun)
    /// </summary>
    public const float AU_IN_KM = 149597870.7f;

    /// <summary>
    /// Speed of light in kilometers per second
    /// </summary>
    public const float LIGHT_SPEED_KM_S = 299792.458f;

    /// <summary>
    /// Gravitational constant in m³ kg⁻¹ s⁻²
    /// </summary>
    public const float GRAVITATIONAL_CONSTANT = 6.67430e-11f;

    /// <summary>
    /// Maximum number of planets allowed in a solar system
    /// </summary>
    public const int MAX_PLANETS = 15;

    /// <summary>
    /// Minimum orbital radius in Astronomical Units
    /// </summary>
    public const float MIN_ORBITAL_RADIUS_AU = 0.1f;

    /// <summary>
    /// Maximum orbital radius in Astronomical Units
    /// </summary>
    public const float MAX_ORBITAL_RADIUS_AU = 50f;

    /// <summary>
    /// Base orbital radius for the first planet in game units
    /// </summary>
    public const float BASE_ORBITAL_RADIUS = 150f;

    /// <summary>
    /// Distance increment between planetary orbits in game units
    /// </summary>
    public const float ORBITAL_RADIUS_INCREMENT = 100f;

    /// <summary>
    /// Maximum random variance applied to orbital radius
    /// </summary>
    public const float ORBITAL_RADIUS_VARIANCE = 50f;

    /// <summary>
    /// Base value for orbital speed calculations
    /// </summary>
    public const float ORBITAL_SPEED_BASE = 50f;

    /// <summary>
    /// Minimum orbital radius threshold for division-by-zero prevention
    /// </summary>
    public const float MIN_ORBITAL_RADIUS = 1f;

    /// <summary>
    /// Default orbital speed when radius is below minimum threshold
    /// </summary>
    public const float DEFAULT_ORBITAL_SPEED = 1f;
}