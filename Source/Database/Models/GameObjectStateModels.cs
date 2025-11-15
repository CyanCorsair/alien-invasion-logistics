using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Core.Types.Research;
using Core.Types.SolarSystem;

namespace Core.Database.Models
{
    public class ResourcesState
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int EnergyStored { get; set; }
        public float EnergyIncomeDaily { get; set; }
        public int MineralsStored { get; set; }
        public float MineralsIncomeDaily { get; set; }
    }

    public class ResearchState
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public ResearchItem CurrentResearch { get; set; }
        public List<ResearchItem> ResearchQueue { get; set; } = new();
        public List<ResearchItem> CompletedResearch { get; set; } = new();
    }

    public class SolarSystemState
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? SystemSunId { get; set; }
        public SunStateModel SystemSun { get; set; }

        public List<PlanetarySystemState> PlanetarySystems { get; set; } = new();
        public List<AsteroidBeltState> AsteroidBelts { get; set; } = new();
    }

    public class SunStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DisplayName { get; set; }
        public string SystemName { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Mass { get; set; }
    }

    public class PlanetarySystemState
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "New Planetary System";

        public Guid SolarSystemStateId { get; set; }
        public SolarSystemState SolarSystem { get; set; }

        public List<PlanetStateModel> Planets { get; set; } = new();
    }

    public class PlanetStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string DisplayName { get; set; }
        public string SystemName { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        public float Mass { get; set; }

        public Guid PlanetarySystemId { get; set; }
        public PlanetarySystemState PlanetarySystem { get; set; }
    }

    public class AsteroidBeltState
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "New Asteroid Belt";

        public Guid SolarSystemStateId { get; set; }
        public SolarSystemState SolarSystem { get; set; }
    }
}
