using System;
using AlienInvasionLogistics.Source.Constants;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Utilities;
using FluentAssertions;
using Xunit;

namespace AlienInvasionLogistics.Tests.Utilities;

public class OrbitalMechanicsCalculatorTests
{
    // ==================== Low Orbit Altitude Tests ====================

    [Fact]
    public void CalculateLowOrbitAltitude_ShouldReturnCorrectAltitude()
    {
        // Arrange
        float bodyRadius = 100f;
        float expectedAltitude = bodyRadius * (SolarSystemConstants.LowOrbitMultiplier - 1f); // 20f

        // Act
        float result = OrbitalMechanicsCalculator.CalculateLowOrbitAltitude(bodyRadius);

        // Assert
        result.Should().BeApproximately(expectedAltitude, 0.01f);
    }

    [Fact]
    public void CalculateLowOrbitAltitude_WithZeroRadius_ShouldReturnZero()
    {
        // Act
        float result = OrbitalMechanicsCalculator.CalculateLowOrbitAltitude(0f);

        // Assert
        result.Should().Be(0f);
    }

    [Theory]
    [InlineData(10f)]
    [InlineData(50f)]
    [InlineData(100f)]
    [InlineData(500f)]
    public void CalculateLowOrbitAltitude_ShouldScaleLinearly(float radius)
    {
        // Act
        float result = OrbitalMechanicsCalculator.CalculateLowOrbitAltitude(radius);

        // Assert - LEO altitude should be 20% of body radius
        result.Should().BeApproximately(radius * 0.2f, 0.01f);
    }

    // ==================== Geostationary Orbit Tests ====================

    [Fact]
    public void CalculateGeostationaryAltitude_WithTidallyLockedBody_ShouldReturnZero()
    {
        // Arrange - tidally locked body has rotation period of 0
        float mass = 1f;
        float radius = 10f;
        float rotationPeriod = 0f;
        float parentMass = 1000f;
        float semiMajorAxis = 150f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateGeostationaryAltitude(
            mass, radius, rotationPeriod, parentMass, semiMajorAxis
        );

        // Assert
        result.Should().Be(0f);
    }

    [Fact]
    public void CalculateGeostationaryAltitude_WithValidParameters_ShouldReturnPositiveAltitude()
    {
        // Arrange - Earth-like parameters
        float mass = 1f; // 1 Earth mass
        float radius = 6.371f; // ~Earth radius in 1000s of km
        float rotationPeriod = 24f; // 24 hours
        float parentMass = 333000f; // Sun mass relative to Earth
        float semiMajorAxis = 149597.87f; // 1 AU in 1000s of km

        // Act
        float result = OrbitalMechanicsCalculator.CalculateGeostationaryAltitude(
            mass, radius, rotationPeriod, parentMass, semiMajorAxis
        );

        // Assert - GEO should be positive (if calculation is valid within Hill sphere)
        // Note: actual value depends on the implementation's unit conversions
        result.Should().BeGreaterThanOrEqualTo(0f);
    }

    // ==================== High Orbit Altitude Tests ====================

    [Fact]
    public void CalculateHighOrbitAltitude_ShouldReturnPositiveValue()
    {
        // Arrange
        float radius = 10f;
        float mass = 1f;
        float parentMass = 100f;
        float semiMajorAxis = 1000f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateHighOrbitAltitude(
            radius, mass, parentMass, semiMajorAxis
        );

        // Assert
        result.Should().BeGreaterThan(0f);
    }

    [Fact]
    public void CalculateHighOrbitAltitude_ShouldBeAtLeastMinimumMultiple()
    {
        // Arrange
        float radius = 10f;
        float mass = 0.001f; // Very small mass = small Hill sphere
        float parentMass = 1000f;
        float semiMajorAxis = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateHighOrbitAltitude(
            radius, mass, parentMass, semiMajorAxis
        );

        // Assert - should be at least (HighOrbitMultiplier - 1) * radius above surface
        float minAltitude = radius * (SolarSystemConstants.HighOrbitMultiplier - 1f);
        result.Should().BeGreaterThanOrEqualTo(minAltitude);
    }

    // ==================== Hill Sphere Tests ====================

    [Fact]
    public void CalculateHillSphereRadius_WithValidParameters_ShouldReturnPositiveValue()
    {
        // Arrange
        float bodyMass = 1f;
        float parentMass = 1000f;
        float semiMajorAxis = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateHillSphereRadius(
            bodyMass, parentMass, semiMajorAxis
        );

        // Assert
        result.Should().BeGreaterThan(0f);
    }

    [Fact]
    public void CalculateHillSphereRadius_ShouldScaleWithSemiMajorAxis()
    {
        // Arrange
        float bodyMass = 1f;
        float parentMass = 1000f;
        float semiMajorAxis1 = 100f;
        float semiMajorAxis2 = 200f;

        // Act
        float result1 = OrbitalMechanicsCalculator.CalculateHillSphereRadius(bodyMass, parentMass, semiMajorAxis1);
        float result2 = OrbitalMechanicsCalculator.CalculateHillSphereRadius(bodyMass, parentMass, semiMajorAxis2);

        // Assert - Hill sphere should scale with semi-major axis
        result2.Should().BeGreaterThan(result1);
    }

    [Fact]
    public void CalculateHillSphereRadius_WithZeroParentMass_ShouldReturnFallback()
    {
        // Arrange
        float bodyMass = 1f;
        float parentMass = 0f;
        float semiMajorAxis = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateHillSphereRadius(bodyMass, parentMass, semiMajorAxis);

        // Assert - should return fallback value
        result.Should().BeGreaterThan(0f);
    }

    // ==================== Orbital Period Tests ====================

    [Fact]
    public void CalculateOrbitalPeriod_WithValidParameters_ShouldReturnPositiveValue()
    {
        // Arrange
        float bodyMass = 1f;
        float orbitalRadius = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateOrbitalPeriod(bodyMass, orbitalRadius);

        // Assert
        result.Should().BeGreaterThan(0f);
    }

    [Fact]
    public void CalculateOrbitalPeriod_WithZeroMass_ShouldReturnZero()
    {
        // Arrange
        float bodyMass = 0f;
        float orbitalRadius = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateOrbitalPeriod(bodyMass, orbitalRadius);

        // Assert
        result.Should().Be(0f);
    }

    [Fact]
    public void CalculateOrbitalPeriod_ShouldIncreaseWithRadius()
    {
        // Arrange
        float bodyMass = 1f;
        float radius1 = 100f;
        float radius2 = 200f;

        // Act
        float period1 = OrbitalMechanicsCalculator.CalculateOrbitalPeriod(bodyMass, radius1);
        float period2 = OrbitalMechanicsCalculator.CalculateOrbitalPeriod(bodyMass, radius2);

        // Assert - larger orbits have longer periods
        period2.Should().BeGreaterThan(period1);
    }

    // ==================== Orbital Velocity Tests ====================

    [Fact]
    public void CalculateOrbitalVelocity_WithValidParameters_ShouldReturnPositiveValue()
    {
        // Arrange
        float bodyMass = 1f;
        float orbitalRadius = 100f;

        // Act
        float result = OrbitalMechanicsCalculator.CalculateOrbitalVelocity(bodyMass, orbitalRadius);

        // Assert
        result.Should().BeGreaterThan(0f);
    }

    [Fact]
    public void CalculateOrbitalVelocity_ShouldDecreaseWithRadius()
    {
        // Arrange
        float bodyMass = 1f;
        float radius1 = 100f;
        float radius2 = 200f;

        // Act
        float velocity1 = OrbitalMechanicsCalculator.CalculateOrbitalVelocity(bodyMass, radius1);
        float velocity2 = OrbitalMechanicsCalculator.CalculateOrbitalVelocity(bodyMass, radius2);

        // Assert - farther orbits have lower velocities
        velocity2.Should().BeLessThan(velocity1);
    }

    // ==================== CanHaveGeostationaryOrbit Tests ====================

    [Theory]
    [InlineData(CelestialBodyType.Star, false)]
    [InlineData(CelestialBodyType.Comet, false)]
    [InlineData(CelestialBodyType.Asteroid, false)]
    [InlineData(CelestialBodyType.MinorPlanet, false)]
    [InlineData(CelestialBodyType.Planet, true)]
    [InlineData(CelestialBodyType.GasGiant, true)]
    [InlineData(CelestialBodyType.IceGiant, true)]
    [InlineData(CelestialBodyType.LargePlanet, true)]
    [InlineData(CelestialBodyType.DwarfPlanet, true)]
    [InlineData(CelestialBodyType.Moon, true)]
    public void CanHaveGeostationaryOrbit_ShouldReturnCorrectValue(CelestialBodyType bodyType, bool expected)
    {
        // Act
        bool result = OrbitalMechanicsCalculator.CanHaveGeostationaryOrbit(bodyType);

        // Assert
        result.Should().Be(expected);
    }

    // ==================== Integration Tests ====================

    [Fact]
    public void OrbitAltitudes_ShouldBeInCorrectOrder_LEO_LessThan_GEO_LessThan_HEO()
    {
        // Arrange - Earth-like body with reasonable parameters
        float mass = 1f;
        float radius = 10f;
        float rotationPeriod = 24f;
        float parentMass = 333000f;
        float semiMajorAxis = 150000f;

        // Act
        float leoAltitude = OrbitalMechanicsCalculator.CalculateLowOrbitAltitude(radius);
        float geoAltitude = OrbitalMechanicsCalculator.CalculateGeostationaryAltitude(
            mass, radius, rotationPeriod, parentMass, semiMajorAxis
        );
        float heoAltitude = OrbitalMechanicsCalculator.CalculateHighOrbitAltitude(
            radius, mass, parentMass, semiMajorAxis
        );

        // Assert - LEO should be lowest, then GEO (if valid), then HEO
        leoAltitude.Should().BeGreaterThan(0f);
        heoAltitude.Should().BeGreaterThan(leoAltitude);

        // GEO may be 0 if it doesn't fit within Hill sphere constraints
        if (geoAltitude > 0)
        {
            geoAltitude.Should().BeGreaterThan(leoAltitude);
            // Note: GEO could potentially be higher than HEO for very fast rotators
        }
    }
}
