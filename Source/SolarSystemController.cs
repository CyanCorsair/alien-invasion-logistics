using Godot;
using System;
using Core.Controllers.UI.NewGameScreen;
using Core.Database.Models;

namespace Core.Controllers;

public partial class SolarSystemController() : Node2D
{
    private SolarSystemState _solarSystemState;

    public override void _Ready()
    {
        GenerateSolarSystem();
    }

    private void GenerateSolarSystem()
    { }
}
