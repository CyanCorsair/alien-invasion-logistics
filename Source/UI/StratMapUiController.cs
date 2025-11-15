using Core.Database;
using Godot;
using System;

namespace Core.Controllers.UI.StrategicMap;

public struct StrategicGameData
{
}

public partial class StratMapUiController : Control
{
    private Node2D solarSystemReference;
    private GameDataManager gameDataManager;

    private StrategicGameData currentGameData;

    public override void _Ready()
    {
        GD.Print("StratMapUiController ready.");
    }

    public override void _Process(double delta) { }

    private void LoadGameData() { }

    private void PrepareSolarSystemScene() { }

    private void UpdateUIElements() { }
}
