using Core.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Types.SolarSystem
{
    public class PlanetarySystem
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; } = "New Planetary System";
        public List<Planet> Planets { get; set; } = new List<Planet>();
    }

    public class AsteroidBelt
    {
        [Key]
        public Guid Id { get; set; } = new Guid();
        public string Name { get; set; } = "New Asteroid Belt";
    }
}
