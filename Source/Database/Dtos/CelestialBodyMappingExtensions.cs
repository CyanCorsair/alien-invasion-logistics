using System;
using System.Linq;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Dtos;

/// <summary>
/// Extension methods for mapping between CelestialBody entities and DTOs.
/// </summary>
public static class CelestialBodyMappingExtensions
{
    /// <summary>
    /// Converts a CelestialBody to a PlanetGameObjectDto.
    /// </summary>
    public static PlanetGameObjectDto ToPlanetDto(this CelestialBody body)
    {
        if (body.BodyType == CelestialBodyType.Star)
            throw new InvalidOperationException("Cannot convert Star to PlanetDto. Use ToStarDto() instead.");

        return new PlanetGameObjectDto
        {
            Id = body.Id,
            Name = body.Name,
            PositionX = body.PositionX,
            PositionY = body.PositionY,
            Mass = body.Mass,
            Radius = body.Radius,
            OrbitalPeriod = body.OrbitalPeriod,
            SemiMajorAxis = body.SemiMajorAxis,
            Eccentricity = body.Eccentricity,
            SunlightLevel = body.SunlightLevel,
            BodyType = body.BodyType,
            ParentBodyId = body.ParentBodyId,
            ResourceDeposits = body.ResourceDeposits?.ToList() ?? new(),
            Orbits = body.Orbits?.ToList() ?? new(),
            LandingSites = body.LandingSites?.ToList() ?? new()
        };
    }

    /// <summary>
    /// Converts a CelestialBody to a StarGameObjectDto.
    /// </summary>
    public static StarGameObjectDto ToStarDto(this CelestialBody body)
    {
        if (body.BodyType != CelestialBodyType.Star)
            throw new InvalidOperationException("Cannot convert non-Star to StarDto. Use ToPlanetDto() instead.");

        return new StarGameObjectDto
        {
            Id = body.Id,
            Name = body.Name,
            PositionX = body.PositionX,
            PositionY = body.PositionY,
            Mass = body.Mass,
            Radius = body.Radius,
            BodyType = body.BodyType,
            OrbitalPeriod = body.OrbitalPeriod,
            SemiMajorAxis = body.SemiMajorAxis,
            Eccentricity = body.Eccentricity,
            SiblingBodyIds = body.SiblingBodyIds ?? new(),
            ResourceDeposits = body.ResourceDeposits?.ToList() ?? new(),
            Orbits = body.Orbits?.ToList() ?? new()
        };
    }

    /// <summary>
    /// Converts a PlanetarySystem to DTO.
    /// </summary>
    public static PlanetarySystemGameObjectDto ToDto(this PlanetarySystem system)
    {
        return new PlanetarySystemGameObjectDto
        {
            Id = system.Id,
            Name = system.Name,
            CentralMassId = system.CentralMassId,
            CelestialBodyIds = system.CelestialBodyIds ?? new(),
            PlanetarySystemIds = system.PlanetarySystemIds ?? new()
        };
    }

    /// <summary>
    /// Updates a CelestialBody from a PlanetGameObjectDto.
    /// </summary>
    public static void UpdateFromDto(this CelestialBody body, PlanetGameObjectDto dto)
    {
        body.Name = dto.Name;
        body.PositionX = dto.PositionX;
        body.PositionY = dto.PositionY;
        body.Mass = dto.Mass;
        body.Radius = dto.Radius;
        body.OrbitalPeriod = dto.OrbitalPeriod;
        body.SemiMajorAxis = dto.SemiMajorAxis;
        body.Eccentricity = dto.Eccentricity;
        body.SunlightLevel = dto.SunlightLevel;
        body.BodyType = dto.BodyType;
        body.ParentBodyId = dto.ParentBodyId;
        // Note: Collections (ResourceDeposits, Orbits, LandingSites) should be updated separately
    }

    /// <summary>
    /// Updates a CelestialBody from a StarGameObjectDto.
    /// </summary>
    public static void UpdateFromDto(this CelestialBody body, StarGameObjectDto dto)
    {
        body.Name = dto.Name;
        body.PositionX = dto.PositionX;
        body.PositionY = dto.PositionY;
        body.Mass = dto.Mass;
        body.Radius = dto.Radius;
        body.BodyType = dto.BodyType;
        body.OrbitalPeriod = dto.OrbitalPeriod;
        body.SemiMajorAxis = dto.SemiMajorAxis;
        body.Eccentricity = dto.Eccentricity;
        body.SiblingBodyIds = dto.SiblingBodyIds;
    }
}
