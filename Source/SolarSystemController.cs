using AlienInvasionLogistics.Source.Database.Models;
using Godot;

namespace AlienInvasionLogistics.Source;

public partial class SolarSystemController : Node2D
{
    private SolarSystem _solarSystem;

    public override void _Ready()
    {
        GenerateSolarSystem();
    }

    private void GenerateSolarSystem()
    {
    }
}