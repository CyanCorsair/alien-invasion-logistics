using Godot;

namespace AlienInvasionLogistics.Source.UI;

public struct StrategicGameData
{
}

public partial class StratMapUiController : Control
{
    private StrategicGameData currentGameData;
    private GameDataService gameDataService;
    private Node2D solarSystemReference;

    public override void _Ready()
    {
        GD.Print("StratMapUiController ready.");
    }

    public override void _Process(double delta)
    {
    }

    private void LoadGameData()
    {
    }

    private void PrepareSolarSystemScene()
    {
    }

    private void UpdateUIElements()
    {
    }
}