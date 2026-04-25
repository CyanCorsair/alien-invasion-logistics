using System.Collections.Generic;
using System.Linq;
using AlienInvasionLogistics.Source.Database.Models;

namespace AlienInvasionLogistics.Source.Utilities;

/// <summary>
/// Calculates strategic importance scores for solar systems and celestial bodies.
/// Used for UI display to distinguish major bodies (strategic importance) from minor bodies (resource value).
/// </summary>
public static class SolarSystemScoreCalculator
{
    /// <summary>
    /// Weights for major bodies representing strategic importance.
    /// </summary>
    private static readonly Dictionary<CelestialBodyType, int> MajorBodyWeights = new()
    {
        { CelestialBodyType.Star, 100 },
        { CelestialBodyType.GasGiant, 50 },
        { CelestialBodyType.IceGiant, 45 },
        { CelestialBodyType.LargePlanet, 30 },
        { CelestialBodyType.Planet, 25 },
        { CelestialBodyType.DwarfPlanet, 15 }
    };

    /// <summary>
    /// Weights for minor bodies representing resource value.
    /// </summary>
    private static readonly Dictionary<CelestialBodyType, int> MinorBodyWeights = new()
    {
        { CelestialBodyType.Moon, 10 },
        { CelestialBodyType.MinorPlanet, 5 },
        { CelestialBodyType.Asteroid, 2 },
        { CelestialBodyType.Comet, 3 }
    };

    /// <summary>
    /// Determines if a celestial body type is considered a major body.
    /// </summary>
    /// <param name="bodyType">The celestial body type to check.</param>
    /// <returns>True if the body is a major body (strategic importance), false if minor (resource value).</returns>
    public static bool IsMajorBody(CelestialBodyType bodyType)
    {
        return MajorBodyWeights.ContainsKey(bodyType);
    }

    /// <summary>
    /// Gets the weight/score value for a celestial body type.
    /// </summary>
    /// <param name="bodyType">The celestial body type.</param>
    /// <returns>The weight value for scoring purposes.</returns>
    public static int GetBodyWeight(CelestialBodyType bodyType)
    {
        if (MajorBodyWeights.TryGetValue(bodyType, out int majorWeight))
            return majorWeight;

        if (MinorBodyWeights.TryGetValue(bodyType, out int minorWeight))
            return minorWeight;

        return 0;
    }

    /// <summary>
    /// Calculates the complete score for a solar system.
    /// </summary>
    /// <param name="solarSystem">The solar system to score.</param>
    /// <returns>A SolarSystemScore containing all scoring details.</returns>
    public static SolarSystemScore CalculateSystemScore(SolarSystem solarSystem)
    {
        var score = new SolarSystemScore();

        // Count the central star
        if (solarSystem.CentralMass != null)
        {
            AddBodyToScore(score, solarSystem.CentralMass.BodyType);
        }

        // Process all planetary systems
        if (solarSystem.PlanetarySystems != null)
        {
            foreach (var planetarySystem in solarSystem.PlanetarySystems)
            {
                if (planetarySystem.CentralMass != null)
                    AddBodyToScore(score, planetarySystem.CentralMass.BodyType);

                if (planetarySystem.CelestialBodies != null)
                    foreach (var body in planetarySystem.CelestialBodies)
                        AddBodyToScore(score, body.BodyType);
            }
        }

        // Asteroid belts and comet clouds contribute to minor body score
        if (solarSystem.AsteroidBelts != null)
        {
            score.AsteroidBeltCount = solarSystem.AsteroidBelts.Count;
            score.MinorBodyScore += score.AsteroidBeltCount * 20;
        }

        if (solarSystem.CometClouds != null)
        {
            score.CometCloudCount = solarSystem.CometClouds.Count;
            score.MinorBodyScore += score.CometCloudCount * 15;
        }

        return score;
    }

    private static void AddBodyToScore(SolarSystemScore score, CelestialBodyType bodyType)
    {
        // Update counts by type using TryGetValue pattern
        score.BodyCountsByType.TryGetValue(bodyType, out int currentCount);
        score.BodyCountsByType[bodyType] = currentCount + 1;

        // Update major/minor counts and scores
        if (IsMajorBody(bodyType))
        {
            score.MajorBodyCount++;
            score.MajorBodyScore += GetBodyWeight(bodyType);
        }
        else
        {
            score.MinorBodyCount++;
            score.MinorBodyScore += GetBodyWeight(bodyType);
        }
    }
}

/// <summary>
/// Represents the calculated score for a solar system.
/// </summary>
public class SolarSystemScore
{
    /// <summary>
    /// Number of major bodies (stars, planets, dwarf planets).
    /// </summary>
    public int MajorBodyCount { get; set; }

    /// <summary>
    /// Number of minor bodies (moons, asteroids, comets).
    /// </summary>
    public int MinorBodyCount { get; set; }

    /// <summary>
    /// Total score from major bodies (strategic importance).
    /// </summary>
    public int MajorBodyScore { get; set; }

    /// <summary>
    /// Total score from minor bodies (resource value).
    /// </summary>
    public int MinorBodyScore { get; set; }

    /// <summary>
    /// Combined total score.
    /// </summary>
    public int TotalScore => MajorBodyScore + MinorBodyScore;

    /// <summary>
    /// Breakdown of body counts by type.
    /// </summary>
    public Dictionary<CelestialBodyType, int> BodyCountsByType { get; set; } = new();

    public int AsteroidBeltCount { get; set; }
    public int CometCloudCount { get; set; }
}
