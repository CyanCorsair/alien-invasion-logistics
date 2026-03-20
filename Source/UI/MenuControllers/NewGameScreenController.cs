using System;
using System.Globalization;
using AlienInvasionLogistics.Source.Database;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.Utilities;
using Godot;

namespace AlienInvasionLogistics.Source.UI.MenuControllers;

public enum StarType
{
    MainSequence,
    BlueGiant,
    RedDwarf
}

public enum StartingResourcesMultiplier
{
    Low,
    Medium,
    High
}

public struct GameSettings
{
    public int NumberOfPlanets;
    public StarType StarType;
    public Guid PlayerNationId;
    public int StartingMineralModifier;
    public int StartingEnergyModifier;
    public StartingResources StartingResources;
    public StartingResearch  StartingResearch;
    public int AiPlayerCount;
    public string PlayerName;
    public string SessionName;
    
}

public partial class NewGameScreenController : Panel
{
    private const int MinPlanets = 1;
    private const int MaxPlanets = 20;
    private const int DefaultPlanets = 5;

    private const StarType DefaultStarType = StarType.MainSequence;

    private const StartingResourcesMultiplier DefaultStartingResources =
        StartingResourcesMultiplier.Medium;

    private Button _cancelSetupButton;
    private double _numberOfPlanets = DefaultPlanets;

    private HSlider _planetCountControl;
    private RichTextLabel _planetCountText;
    private LineEdit _playerNameControl;

    private Button _startGameButton;
    private StartingResourcesMultiplier _startingResourcesMultiplier = DefaultStartingResources;
    private OptionButton _startingResourcesMultiplierControl;
    private StarType _starType = DefaultStarType;

    private OptionButton _starTypeControl;

    private int StartingEnergyResources
    {
        get
        {
            switch (_startingResourcesMultiplier)
            {
                case StartingResourcesMultiplier.Low:
                    return 100;
                case StartingResourcesMultiplier.Medium:
                    return 200;
                case StartingResourcesMultiplier.High:
                    return 300;
                default:
                    return 200;
            }
        }
    }

    private int StartingMineralResources
    {
        get
        {
            switch (_startingResourcesMultiplier)
            {
                case StartingResourcesMultiplier.Low:
                    return 1000;
                case StartingResourcesMultiplier.Medium:
                    return 2000;
                case StartingResourcesMultiplier.High:
                    return 3000;
                default:
                    return 2000;
            }
        }
    }

    private string PlayerName { get; set; } = "Player";
    private string SessionName { get; set; } = "New Game";

    public override void _Ready()
    {
        _playerNameControl = GetNode<LineEdit>(
            "SettingsPanel/BasicSettings/PlayerNameContainer/PlayerNameTextField"
        );
        _playerNameControl.Text = PlayerName;

        _planetCountControl = GetNode<HSlider>(
            "SettingsPanel/BasicSettings/PlanetCountContainer/PlanetCountRange"
        );
        _planetCountText = GetNode<RichTextLabel>(
            "SettingsPanel/BasicSettings/PlanetCountContainer/PlanetCountText"
        );

        _starTypeControl = GetNode<OptionButton>(
            "SettingsPanel/BasicSettings/StarTypeCobtainer/StarTypeDropdown/StarTypeOptions"
        );
        _startingResourcesMultiplierControl = GetNode<OptionButton>(
            "SettingsPanel/BasicSettings/StartingResourcesContainer/StartingResourcesDropdown/StartingResourceOptions"
        );

        _startGameButton = GetNode<Button>("ControlsPanel/ControlsContainer/StartGameButton");
        _cancelSetupButton = GetNode<Button>("ControlsPanel/ControlsContainer/CancelStartButton");

        _planetCountControl.ValueChanged += OnPlanetCountChanged;
        _starTypeControl.ItemSelected += OnStarTypeChanged;
        _startingResourcesMultiplierControl.ItemSelected += OnStartingResourcesMultiplierChanged;

        _startGameButton.Pressed += OnStartNewGame;
        _cancelSetupButton.Pressed += OnCancelSetup;
    }

    public override void _Process(double delta)
    {
    }

    public override void _ExitTree()
    {
        _planetCountControl.ValueChanged -= OnPlanetCountChanged;
        _starTypeControl.ItemSelected -= OnStarTypeChanged;
        _startingResourcesMultiplierControl.ItemSelected -= OnStartingResourcesMultiplierChanged;

        _startGameButton.Pressed -= OnStartNewGame;
        _cancelSetupButton.Pressed -= OnCancelSetup;
    }

    private void OnPlanetCountChanged(double newPlanetCount)
    {
        if (newPlanetCount < MinPlanets || newPlanetCount > MaxPlanets)
        {
            GD.PrintErr(
                $"Planet count {newPlanetCount} is out of bounds. It must be between {MinPlanets} and {MaxPlanets}."
            );
            return;
        }

        _numberOfPlanets = newPlanetCount;
        _planetCountText.Text = _numberOfPlanets.ToString(CultureInfo.InvariantCulture);
    }

    private void OnStarTypeChanged(long index)
    {
        _starTypeControl.Selected = (int)index;
        _starType = (StarType)_starTypeControl.Selected;
    }

    private void OnStartingResourcesMultiplierChanged(long index)
    {
        _startingResourcesMultiplier = (StartingResourcesMultiplier)
            _startingResourcesMultiplierControl.Selected;
    }

    private async void OnStartNewGame()
    {
        PlayerName = _playerNameControl.Text;
        _numberOfPlanets = _planetCountControl.Value;
        _starType = (StarType)_starTypeControl.Selected;
        _startingResourcesMultiplier = (StartingResourcesMultiplier)
            _startingResourcesMultiplierControl.Selected;

        GD.Print(
            $"Starting new game with {_numberOfPlanets} planets, star type {_starType}, starting resources: energy {StartingEnergyResources} | mineral {StartingMineralResources}, player name {PlayerName}"
        );

        StartingResearch startingResearch = new();
        StartingResources startingResources =
            new() { Energy = StartingEnergyResources, Minerals = StartingMineralResources };

        GameSettings settings =
            new()
            {
                NumberOfPlanets = (int)_numberOfPlanets,
                StarType = _starType,
                PlayerName = PlayerName,
                SessionName = SessionName,
                StartingResearch = startingResearch,
                StartingResources = startingResources
            };

        try
        {
            GD.Print("Creating new game data");
            var gameDataService = ServiceContainer.GetRequiredService<IGameDataService>();

#if DEBUG
            // Clear database for new game (TODO: remove in production)
            await gameDataService.ClearDatabaseAsync();
#endif

            // Create new game asynchronously
            await gameDataService.CreateNewGameAsync(settings);

            ChangeToGameScene();
        }
        catch (Exception ex)
        {
            ErrorHandler.LogMessage("Error creating new game", ex);
            ShowErrorDialog("Failed to create new game. Please try again.");
        }
    }

    private void ChangeToGameScene()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainGameScene.tscn");
    }

    private void ShowErrorDialog(string message)
    {
        var dialog = new AcceptDialog();
        dialog.DialogText = message;
        dialog.Title = "Error";
        dialog.Size = new Vector2I(400, 150);
        AddChild(dialog);
        dialog.PopupCentered();
    }

    private void OnCancelSetup()
    {
        GetTree().ChangeSceneToFile("res://Scenes/MainMenuScene.tscn");
    }
}