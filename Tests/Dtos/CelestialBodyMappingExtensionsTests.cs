using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Dtos;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace AlienInvasionLogistics.Tests.Dtos;

public class ToPlanetDtoTests
{
    [Fact]
    public void ToPlanetDto_ValidPlanet_ReturnsDto()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");

        var dto = planet.ToPlanetDto();

        dto.Should().NotBeNull();
        dto.Id.Should().Be(planet.Id);
        dto.Name.Should().Be("Earth");
        dto.BodyType.Should().Be(CelestialBodyType.Planet);
    }

    [Fact]
    public void ToPlanetDto_ValidMoon_ReturnsDto()
    {
        var parentId = Guid.NewGuid();
        var moon = TestDataBuilder.CreateMoon("Luna", parentId);

        var dto = moon.ToPlanetDto();

        dto.Should().NotBeNull();
        dto.BodyType.Should().Be(CelestialBodyType.Moon);
        dto.ParentBodyId.Should().Be(parentId);
    }

    [Fact]
    public void ToPlanetDto_ValidAsteroid_ReturnsDto()
    {
        var asteroid = TestDataBuilder.CreatePlanet("Ceres", CelestialBodyType.Asteroid);

        var dto = asteroid.ToPlanetDto();

        dto.Should().NotBeNull();
        dto.BodyType.Should().Be(CelestialBodyType.Asteroid);
    }

    [Fact]
    public void ToPlanetDto_Star_ThrowsInvalidOperationException()
    {
        var star = TestDataBuilder.CreateStar("Sol");

        Action act = () => star.ToPlanetDto();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Star*PlanetDto*ToStarDto*");
    }

    [Fact]
    public void ToPlanetDto_PreservesAllProperties()
    {
        var planet = new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = "Test Planet",
            BodyType = CelestialBodyType.Planet,
            PositionX = 100f,
            PositionY = 200f,
            Mass = 1000f,
            Radius = 500f,
            OrbitalPeriod = 365f,
            SemiMajorAxis = 149f,
            Eccentricity = 0.017f,
            SunlightLevel = 100,
            ParentBodyId = Guid.NewGuid()
        };

        var dto = planet.ToPlanetDto();

        dto.Id.Should().Be(planet.Id);
        dto.Name.Should().Be(planet.Name);
        dto.BodyType.Should().Be(planet.BodyType);
        dto.PositionX.Should().Be(planet.PositionX);
        dto.PositionY.Should().Be(planet.PositionY);
        dto.Mass.Should().Be(planet.Mass);
        dto.Radius.Should().Be(planet.Radius);
        dto.OrbitalPeriod.Should().Be(planet.OrbitalPeriod);
        dto.SemiMajorAxis.Should().Be(planet.SemiMajorAxis);
        dto.Eccentricity.Should().Be(planet.Eccentricity);
        dto.SunlightLevel.Should().Be(planet.SunlightLevel);
        dto.ParentBodyId.Should().Be(planet.ParentBodyId);
    }

    [Fact]
    public void ToPlanetDto_NullResourceDeposits_ReturnsEmptyList()
    {
        var planet = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Planet,
            ResourceDeposits = null
        };

        var dto = planet.ToPlanetDto();

        dto.ResourceDeposits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToPlanetDto_NullOrbits_ReturnsEmptyList()
    {
        var planet = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Planet,
            Orbits = null
        };

        var dto = planet.ToPlanetDto();

        dto.Orbits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToPlanetDto_NullLandingSites_ReturnsEmptyList()
    {
        var planet = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Planet,
            LandingSites = null
        };

        var dto = planet.ToPlanetDto();

        dto.LandingSites.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToPlanetDto_WithResources_PreservesResources()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        planet.ResourceDeposits = new List<GameResource>
        {
            TestDataBuilder.CreateMineralResource(500),
            TestDataBuilder.CreateEnergyResource(200)
        };

        var dto = planet.ToPlanetDto();

        dto.ResourceDeposits.Should().HaveCount(2);
    }

    [Fact]
    public void ToPlanetDto_WithOrbits_PreservesOrbits()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        planet.Orbits = new List<Orbit>
        {
            TestDataBuilder.CreateOrbit("Low Orbit"),
            TestDataBuilder.CreateOrbit("High Orbit")
        };

        var dto = planet.ToPlanetDto();

        dto.Orbits.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(CelestialBodyType.GiantPlanet)]
    [InlineData(CelestialBodyType.LargePlanet)]
    [InlineData(CelestialBodyType.Planet)]
    [InlineData(CelestialBodyType.DwarfPlanet)]
    [InlineData(CelestialBodyType.MinorPlanet)]
    [InlineData(CelestialBodyType.Moon)]
    [InlineData(CelestialBodyType.Asteroid)]
    [InlineData(CelestialBodyType.Comet)]
    public void ToPlanetDto_NonStarTypes_ShouldSucceed(CelestialBodyType bodyType)
    {
        var body = new BaseNaturalSolarObject { BodyType = bodyType };

        var dto = body.ToPlanetDto();

        dto.BodyType.Should().Be(bodyType);
    }
}

public class ToStarDtoTests
{
    [Fact]
    public void ToStarDto_ValidStar_ReturnsDto()
    {
        var star = TestDataBuilder.CreateStar("Sol");

        var dto = star.ToStarDto();

        dto.Should().NotBeNull();
        dto.Id.Should().Be(star.Id);
        dto.Name.Should().Be("Sol");
        dto.BodyType.Should().Be(CelestialBodyType.Star);
    }

    [Fact]
    public void ToStarDto_Planet_ThrowsInvalidOperationException()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");

        Action act = () => planet.ToStarDto();

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*non-Star*StarDto*ToPlanetDto*");
    }

    [Fact]
    public void ToStarDto_Moon_ThrowsInvalidOperationException()
    {
        var moon = TestDataBuilder.CreateMoon("Luna", Guid.NewGuid());

        Action act = () => moon.ToStarDto();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ToStarDto_PreservesAllProperties()
    {
        var siblingId = Guid.NewGuid();
        var star = new BaseNaturalSolarObject
        {
            Id = Guid.NewGuid(),
            Name = "Sirius A",
            BodyType = CelestialBodyType.Star,
            PositionX = 0f,
            PositionY = 0f,
            Mass = 2000000f,
            Radius = 1200000f,
            OrbitalPeriod = 50f,
            SemiMajorAxis = 20f,
            Eccentricity = 0.5f,
            SiblingBodyIds = new List<Guid> { siblingId }
        };

        var dto = star.ToStarDto();

        dto.Id.Should().Be(star.Id);
        dto.Name.Should().Be(star.Name);
        dto.BodyType.Should().Be(CelestialBodyType.Star);
        dto.PositionX.Should().Be(star.PositionX);
        dto.PositionY.Should().Be(star.PositionY);
        dto.Mass.Should().Be(star.Mass);
        dto.Radius.Should().Be(star.Radius);
        dto.OrbitalPeriod.Should().Be(star.OrbitalPeriod);
        dto.SemiMajorAxis.Should().Be(star.SemiMajorAxis);
        dto.Eccentricity.Should().Be(star.Eccentricity);
        dto.SiblingBodyIds.Should().Contain(siblingId);
    }

    [Fact]
    public void ToStarDto_NullSiblingBodyIds_ReturnsEmptyList()
    {
        var star = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Star,
            SiblingBodyIds = null
        };

        var dto = star.ToStarDto();

        dto.SiblingBodyIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToStarDto_NullResourceDeposits_ReturnsEmptyList()
    {
        var star = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Star,
            ResourceDeposits = null
        };

        var dto = star.ToStarDto();

        dto.ResourceDeposits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToStarDto_NullOrbits_ReturnsEmptyList()
    {
        var star = new BaseNaturalSolarObject
        {
            BodyType = CelestialBodyType.Star,
            Orbits = null
        };

        var dto = star.ToStarDto();

        dto.Orbits.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToStarDto_BinaryStar_PreservesSiblings()
    {
        var siblingId1 = Guid.NewGuid();
        var siblingId2 = Guid.NewGuid();
        var star = TestDataBuilder.CreateStar("Alpha Centauri A");
        star.SiblingBodyIds = new List<Guid> { siblingId1, siblingId2 };

        var dto = star.ToStarDto();

        dto.SiblingBodyIds.Should().HaveCount(2);
        dto.SiblingBodyIds.Should().Contain(siblingId1);
        dto.SiblingBodyIds.Should().Contain(siblingId2);
    }
}

public class PlanetarySystemToDtoTests
{
    [Fact]
    public void ToDto_ValidSystem_ReturnsDto()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        var system = TestDataBuilder.CreatePlanetarySystem(planet);

        var dto = system.ToDto();

        dto.Should().NotBeNull();
        dto.Id.Should().Be(system.Id);
        dto.Name.Should().Contain("Earth");
        dto.CentralMassId.Should().Be(planet.Id);
    }

    [Fact]
    public void ToDto_PreservesProperties()
    {
        var centralMassId = Guid.NewGuid();
        var bodyId = Guid.NewGuid();
        var subSystemId = Guid.NewGuid();

        var system = new PlanetarySystem
        {
            Id = Guid.NewGuid(),
            Name = "Jupiter System",
            CentralMassId = centralMassId,
            CelestialBodyIds = new List<Guid> { bodyId },
            PlanetarySystemIds = new List<Guid> { subSystemId }
        };

        var dto = system.ToDto();

        dto.Id.Should().Be(system.Id);
        dto.Name.Should().Be("Jupiter System");
        dto.CentralMassId.Should().Be(centralMassId);
        dto.CelestialBodyIds.Should().Contain(bodyId);
        dto.PlanetarySystemIds.Should().Contain(subSystemId);
    }

    [Fact]
    public void ToDto_NullCelestialBodyIds_ReturnsEmptyList()
    {
        var system = new PlanetarySystem
        {
            CelestialBodyIds = null
        };

        var dto = system.ToDto();

        dto.CelestialBodyIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToDto_NullPlanetarySystemIds_ReturnsEmptyList()
    {
        var system = new PlanetarySystem
        {
            PlanetarySystemIds = null
        };

        var dto = system.ToDto();

        dto.PlanetarySystemIds.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ToDto_SystemWithMoons_PreservesBodyIds()
    {
        var planet = TestDataBuilder.CreatePlanet("Earth");
        var system = TestDataBuilder.CreatePlanetarySystem(planet);
        var moonId = Guid.NewGuid();
        system.CelestialBodyIds.Add(moonId);

        var dto = system.ToDto();

        dto.CelestialBodyIds.Should().Contain(moonId);
    }
}

public class UpdateFromPlanetDtoTests
{
    [Fact]
    public void UpdateFromDto_Planet_UpdatesName()
    {
        var body = TestDataBuilder.CreatePlanet("Original");
        var dto = TestDataBuilder.CreatePlanetDto();
        dto.Name = "Updated";

        body.UpdateFromDto(dto);

        body.Name.Should().Be("Updated");
    }

    [Fact]
    public void UpdateFromDto_Planet_UpdatesPosition()
    {
        var body = TestDataBuilder.CreatePlanet("Test");
        var dto = TestDataBuilder.CreatePlanetDto();
        dto.PositionX = 999f;
        dto.PositionY = 888f;

        body.UpdateFromDto(dto);

        body.PositionX.Should().Be(999f);
        body.PositionY.Should().Be(888f);
    }

    [Fact]
    public void UpdateFromDto_Planet_UpdatesBodyType()
    {
        var body = TestDataBuilder.CreatePlanet("Test");
        var dto = TestDataBuilder.CreatePlanetDto();
        dto.BodyType = CelestialBodyType.DwarfPlanet;

        body.UpdateFromDto(dto);

        body.BodyType.Should().Be(CelestialBodyType.DwarfPlanet);
    }

    [Fact]
    public void UpdateFromDto_Planet_UpdatesPhysicalProperties()
    {
        var body = TestDataBuilder.CreatePlanet("Test");
        var dto = TestDataBuilder.CreatePlanetDto();
        dto.Mass = 5000f;
        dto.Radius = 6371f;
        dto.OrbitalPeriod = 365.25f;
        dto.SemiMajorAxis = 149600000f;
        dto.Eccentricity = 0.017f;
        dto.SunlightLevel = 100;

        body.UpdateFromDto(dto);

        body.Mass.Should().Be(5000f);
        body.Radius.Should().Be(6371f);
        body.OrbitalPeriod.Should().Be(365.25f);
        body.SemiMajorAxis.Should().Be(149600000f);
        body.Eccentricity.Should().Be(0.017f);
        body.SunlightLevel.Should().Be(100);
    }

    [Fact]
    public void UpdateFromDto_Planet_UpdatesParentBodyId()
    {
        var body = TestDataBuilder.CreatePlanet("Test");
        var dto = TestDataBuilder.CreatePlanetDto();
        var newParentId = Guid.NewGuid();
        dto.ParentBodyId = newParentId;

        body.UpdateFromDto(dto);

        body.ParentBodyId.Should().Be(newParentId);
    }
}

public class UpdateFromStarDtoTests
{
    [Fact]
    public void UpdateFromDto_Star_UpdatesName()
    {
        var body = TestDataBuilder.CreateStar("Original");
        var dto = TestDataBuilder.CreateStarDto();
        dto.Name = "Updated Star";

        body.UpdateFromDto(dto);

        body.Name.Should().Be("Updated Star");
    }

    [Fact]
    public void UpdateFromDto_Star_UpdatesAllProperties()
    {
        var body = TestDataBuilder.CreateStar("Test");
        var dto = new StarGameObjectDto
        {
            Name = "New Star",
            PositionX = 100f,
            PositionY = 200f,
            Mass = 2000000f,
            Radius = 1500000f,
            BodyType = CelestialBodyType.Star,
            OrbitalPeriod = 50f,
            SemiMajorAxis = 20f,
            Eccentricity = 0.3f,
            SiblingBodyIds = new List<Guid> { Guid.NewGuid() }
        };

        body.UpdateFromDto(dto);

        body.Name.Should().Be("New Star");
        body.PositionX.Should().Be(100f);
        body.PositionY.Should().Be(200f);
        body.Mass.Should().Be(2000000f);
        body.Radius.Should().Be(1500000f);
        body.BodyType.Should().Be(CelestialBodyType.Star);
        body.OrbitalPeriod.Should().Be(50f);
        body.SemiMajorAxis.Should().Be(20f);
        body.Eccentricity.Should().Be(0.3f);
    }

    [Fact]
    public void UpdateFromDto_Star_UpdatesSiblingBodyIds()
    {
        var body = TestDataBuilder.CreateStar("Test");
        var dto = TestDataBuilder.CreateStarDto();
        var siblingId = Guid.NewGuid();
        dto.SiblingBodyIds = new List<Guid> { siblingId };

        body.UpdateFromDto(dto);

        body.SiblingBodyIds.Should().Contain(siblingId);
    }

    [Fact]
    public void UpdateFromDto_Star_ReplacesExistingSiblings()
    {
        var body = TestDataBuilder.CreateStar("Test");
        body.SiblingBodyIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var dto = TestDataBuilder.CreateStarDto();
        var newSiblingId = Guid.NewGuid();
        dto.SiblingBodyIds = new List<Guid> { newSiblingId };

        body.UpdateFromDto(dto);

        body.SiblingBodyIds.Should().HaveCount(1);
        body.SiblingBodyIds.Should().Contain(newSiblingId);
    }
}
