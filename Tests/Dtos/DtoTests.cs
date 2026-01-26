using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Dtos;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace AlienInvasionLogistics.Tests.Dtos;

public class PlanetGameObjectDtoTests
{
    [Fact]
    public void PlanetGameObjectDto_DefaultValues_ShouldBeSet()
    {
        var dto = new PlanetGameObjectDto();

        dto.Id.Should().Be(Guid.Empty);
        dto.Name.Should().Be(string.Empty);
        dto.PositionX.Should().Be(0);
        dto.PositionY.Should().Be(0);
        dto.Mass.Should().Be(0);
        dto.Radius.Should().Be(0);
        dto.OrbitalPeriod.Should().Be(0);
        dto.SemiMajorAxis.Should().Be(0);
        dto.Eccentricity.Should().Be(0);
        dto.SunlightLevel.Should().Be(0);
        dto.ParentBodyId.Should().BeNull();
    }

    [Fact]
    public void PlanetGameObjectDto_EmptyGuid_ByDefault()
    {
        var dto = new PlanetGameObjectDto();

        dto.Id.Should().Be(Guid.Empty);
    }

    [Fact]
    public void PlanetGameObjectDto_ResourceDeposits_EmptyListByDefault()
    {
        var dto = new PlanetGameObjectDto();

        dto.ResourceDeposits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PlanetGameObjectDto_Orbits_EmptyListByDefault()
    {
        var dto = new PlanetGameObjectDto();

        dto.Orbits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PlanetGameObjectDto_LandingSites_EmptyListByDefault()
    {
        var dto = new PlanetGameObjectDto();

        dto.LandingSites.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PlanetGameObjectDto_PropertyAccess_ShouldWork()
    {
        var id = Guid.NewGuid();
        var parentId = Guid.NewGuid();

        var dto = new PlanetGameObjectDto
        {
            Id = id,
            Name = "Earth",
            PositionX = 100f,
            PositionY = 200f,
            Mass = 5000f,
            Radius = 6371f,
            OrbitalPeriod = 365.25f,
            SemiMajorAxis = 149600000f,
            Eccentricity = 0.017f,
            SunlightLevel = 100,
            BodyType = CelestialBodyType.Planet,
            ParentBodyId = parentId
        };

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Earth");
        dto.PositionX.Should().Be(100f);
        dto.PositionY.Should().Be(200f);
        dto.Mass.Should().Be(5000f);
        dto.Radius.Should().Be(6371f);
        dto.OrbitalPeriod.Should().Be(365.25f);
        dto.SemiMajorAxis.Should().Be(149600000f);
        dto.Eccentricity.Should().Be(0.017f);
        dto.SunlightLevel.Should().Be(100);
        dto.BodyType.Should().Be(CelestialBodyType.Planet);
        dto.ParentBodyId.Should().Be(parentId);
    }

    [Fact]
    public void PlanetGameObjectDto_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var dto = TestDataBuilder.CreatePlanetDto();

        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("Test Planet");
        dto.BodyType.Should().Be(CelestialBodyType.Planet);
        dto.ResourceDeposits.Should().NotBeNull();
        dto.Orbits.Should().NotBeNull();
        dto.LandingSites.Should().NotBeNull();
    }

    [Fact]
    public void PlanetGameObjectDto_Collections_CanBeModified()
    {
        var dto = new PlanetGameObjectDto();
        var resource = TestDataBuilder.CreateMineralResource();
        var orbit = TestDataBuilder.CreateOrbit();
        var landingSite = TestDataBuilder.CreateLandingSite();

        dto.ResourceDeposits.Add(resource);
        dto.Orbits.Add(orbit);
        dto.LandingSites.Add(landingSite);

        dto.ResourceDeposits.Should().HaveCount(1);
        dto.Orbits.Should().HaveCount(1);
        dto.LandingSites.Should().HaveCount(1);
    }
}

public class StarGameObjectDtoTests
{
    [Fact]
    public void StarGameObjectDto_DefaultValues_ShouldBeSet()
    {
        var dto = new StarGameObjectDto();

        dto.Id.Should().Be(Guid.Empty);
        dto.Name.Should().Be(string.Empty);
        dto.PositionX.Should().Be(0);
        dto.PositionY.Should().Be(0);
        dto.Mass.Should().Be(0);
        dto.Radius.Should().Be(0);
    }

    [Fact]
    public void StarGameObjectDto_SiblingBodyIds_EmptyListByDefault()
    {
        var dto = new StarGameObjectDto();

        dto.SiblingBodyIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void StarGameObjectDto_ResourceDeposits_EmptyListByDefault()
    {
        var dto = new StarGameObjectDto();

        dto.ResourceDeposits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void StarGameObjectDto_Orbits_EmptyListByDefault()
    {
        var dto = new StarGameObjectDto();

        dto.Orbits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void StarGameObjectDto_PropertyAccess_ShouldWork()
    {
        var id = Guid.NewGuid();
        var siblingId = Guid.NewGuid();

        var dto = new StarGameObjectDto
        {
            Id = id,
            Name = "Sol",
            PositionX = 0f,
            PositionY = 0f,
            Mass = 1989000f,
            Radius = 696340f,
            BodyType = CelestialBodyType.Star,
            OrbitalPeriod = 0f,
            SemiMajorAxis = 0f,
            Eccentricity = 0f,
            SiblingBodyIds = new List<Guid> { siblingId }
        };

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Sol");
        dto.Mass.Should().Be(1989000f);
        dto.BodyType.Should().Be(CelestialBodyType.Star);
        dto.SiblingBodyIds.Should().Contain(siblingId);
    }

    [Fact]
    public void StarGameObjectDto_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var dto = TestDataBuilder.CreateStarDto();

        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("Test Star");
        dto.BodyType.Should().Be(CelestialBodyType.Star);
        dto.SiblingBodyIds.Should().NotBeNull();
        dto.ResourceDeposits.Should().NotBeNull();
        dto.Orbits.Should().NotBeNull();
    }

    [Fact]
    public void StarGameObjectDto_SiblingBodyIds_CanBeModified()
    {
        var dto = new StarGameObjectDto();
        var siblingId = Guid.NewGuid();

        dto.SiblingBodyIds.Add(siblingId);

        dto.SiblingBodyIds.Should().HaveCount(1);
        dto.SiblingBodyIds.Should().Contain(siblingId);
    }
}

public class PlanetarySystemGameObjectDtoTests
{
    [Fact]
    public void PlanetarySystemGameObjectDto_DefaultValues_ShouldBeSet()
    {
        var dto = new PlanetarySystemGameObjectDto();

        dto.Id.Should().Be(Guid.Empty);
        dto.Name.Should().Be(string.Empty);
        dto.CentralMassId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void PlanetarySystemGameObjectDto_CelestialBodyIds_EmptyListByDefault()
    {
        var dto = new PlanetarySystemGameObjectDto();

        dto.CelestialBodyIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PlanetarySystemGameObjectDto_PlanetarySystemIds_EmptyListByDefault()
    {
        var dto = new PlanetarySystemGameObjectDto();

        dto.PlanetarySystemIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void PlanetarySystemGameObjectDto_PropertyAccess_ShouldWork()
    {
        var id = Guid.NewGuid();
        var centralMassId = Guid.NewGuid();
        var bodyId = Guid.NewGuid();
        var subSystemId = Guid.NewGuid();

        var dto = new PlanetarySystemGameObjectDto
        {
            Id = id,
            Name = "Earth System",
            CentralMassId = centralMassId,
            CelestialBodyIds = new List<Guid> { bodyId },
            PlanetarySystemIds = new List<Guid> { subSystemId }
        };

        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Earth System");
        dto.CentralMassId.Should().Be(centralMassId);
        dto.CelestialBodyIds.Should().Contain(bodyId);
        dto.PlanetarySystemIds.Should().Contain(subSystemId);
    }

    [Fact]
    public void PlanetarySystemGameObjectDto_CreatedWithBuilder_ShouldHaveCorrectValues()
    {
        var dto = TestDataBuilder.CreatePlanetarySystemDto();

        dto.Id.Should().NotBe(Guid.Empty);
        dto.Name.Should().Be("Test Planetary System");
        dto.CentralMassId.Should().NotBe(Guid.Empty);
        dto.CelestialBodyIds.Should().NotBeNull();
        dto.PlanetarySystemIds.Should().NotBeNull();
    }

    [Fact]
    public void PlanetarySystemGameObjectDto_Collections_CanBeModified()
    {
        var dto = new PlanetarySystemGameObjectDto();
        var bodyId = Guid.NewGuid();
        var systemId = Guid.NewGuid();

        dto.CelestialBodyIds.Add(bodyId);
        dto.PlanetarySystemIds.Add(systemId);

        dto.CelestialBodyIds.Should().HaveCount(1);
        dto.PlanetarySystemIds.Should().HaveCount(1);
    }
}
