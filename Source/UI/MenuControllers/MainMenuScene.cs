using Godot;

namespace AlienInvasionLogistics.Source.UI.MenuControllers;

public partial class MainMenuScene : Control
{
    private Button _exitGameButton;
    private Button _newGameButton;

    public override void _Ready()
    {
        _newGameButton = GetNode<Button>("Background/MenuButtons/NewGameButton");
        _exitGameButton = GetNode<Button>("Background/MenuButtons/ExitButton");

        _newGameButton.Pressed += OnNewGameButtonPressed;
        _exitGameButton.Pressed += OnExitGameButtonPressed;
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