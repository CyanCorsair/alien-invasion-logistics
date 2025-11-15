using Godot;
using System;

namespace Core.Controllers.UI.MainMenuScreen;

public partial class MainMenuController : Control
{

    Button NewGameButton;
    Button ExitGameButton;

    public override void _Ready()
    {
        NewGameButton = GetNode<Button>("Background/MenuButtons/NewGameButton");
        ExitGameButton = GetNode<Button>("Background/MenuButtons/ExitButton");

        NewGameButton.Pressed += OnNewGameButtonPressed;
        ExitGameButton.Pressed += OnExitGameButtonPressed;
    }

    private void OnNewGameButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MenuScreens/NewGameScreen.tscn");
    }

    private void OnExitGameButtonPressed()
    {
        GD.Print("Exit Game button pressed");
        GetTree().Quit();
    }
}
