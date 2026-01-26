using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Interfaces;

namespace AlienInvasionLogistics.Source.Services;

/// <summary>
/// Service for generating solar system configurations.
/// </summary>
public interface ISolarSystemGenerator
{
    /// <summary>
    /// Generates a new solar system state based on starting settings.
    /// </summary>
    /// <param name="settings">The starting settings for the solar system generation.</param>
    /// <returns>A generated SolarSystem with celestial bodies and configurations.</returns>
    SolarSystem GenerateSolarSystemState(IStartingSettings settings);
}
