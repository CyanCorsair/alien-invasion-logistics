using Godot;
using System;
using System.Threading.Tasks;
using Core.Database;
using Core.Types.Resource;
using Core.Types.Research;
using Core.Utilities;

namespace Core.Controllers.UI.NewGameScreen;

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
    public int StarType;
    public StartingResources StartingResources;
    public StartingResearch StartingResearch;
    public string PlayerName;
}

public partial class NewGameScreenController : Panel
{
    LineEdit playerNameControl;

    HSlider planetCountControl;
    RichTextLabel planetCountText;

    OptionButton starTypeControl;
    OptionButton startingResourcesMultiplierControl;

    Button StartGameButton;
    Button CancelSetupButton;

    private const int MIN_PLANETS = 1;
    private const int MAX_PLANETS = 20;
    private const int DEFAULT_PLANETS = 5;

    private const StarType DEFAULT_STAR_TYPE = StarType.MainSequence;
    private const StartingResourcesMultiplier DEFAULT_STARTING_RESOURCES =
        StartingResourcesMultiplier.Medium;

    string _playerName = "Player";
    double numberOfPlanets = DEFAULT_PLANETS;
    StarType starType = DEFAULT_STAR_TYPE;
    StartingResourcesMultiplier startingResourcesMultiplier = DEFAULT_STARTING_RESOURCES;

    private int StartingEnergyResources
    {
        get
        {
            switch (startingResourcesMultiplier)
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
            switch (startingResourcesMultiplier)
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

    private string PlayerName
    {
        get { return _playerName; }
        set { _playerName = value; }
    }

    public override void _Ready()
    {
        playerNameControl = GetNode<LineEdit>(
            "SettingsPanel/BasicSettings/PlayerNameContainer/PlayerNameTextField"
        );

        planetCountControl = GetNode<HSlider>(
            "SettingsPanel/BasicSettings/PlanetCountContainer/PlanetCountRange"
        );
        planetCountText = GetNode<RichTextLabel>(
            "SettingsPanel/BasicSettings/PlanetCountContainer/PlanetCountText"
        );

        starTypeControl = GetNode<OptionButton>(
            "SettingsPanel/BasicSettings/StarTypeCobtainer/StarTypeDropdown/StarTypeOptions"
        );
        startingResourcesMultiplierControl = GetNode<OptionButton>(
            "SettingsPanel/BasicSettings/StartingResourcesContainer/StartingResourcesDropdown/StartingResourceOptions"
        );

        StartGameButton = GetNode<Button>("ControlsPanel/ControlsContainer/StartGameButton");
        CancelSetupButton = GetNode<Button>("ControlsPanel/ControlsContainer/CancelStartButton");

        planetCountControl.ValueChanged += OnPlanetCountChanged;
        starTypeControl.ItemSelected += OnStarTypeChanged;
        startingResourcesMultiplierControl.ItemSelected += OnStartingResourcesMultiplierChanged;

        StartGameButton.Pressed += OnStartNewGame;
        CancelSetupButton.Pressed += OnCancelSetup;
    }

    public override void _Process(double delta) { }

    public override void _ExitTree()
    {
        planetCountControl.ValueChanged -= OnPlanetCountChanged;
        starTypeControl.ItemSelected -= OnStarTypeChanged;
        startingResourcesMultiplierControl.ItemSelected -= OnStartingResourcesMultiplierChanged;

        StartGameButton.Pressed -= OnStartNewGame;
        CancelSetupButton.Pressed -= OnCancelSetup;
    }

    private void OnPlanetCountChanged(double newPlanetCount)
    {
        if (newPlanetCount < MIN_PLANETS || newPlanetCount > MAX_PLANETS)
        {
            GD.PrintErr(
                $"Planet count {newPlanetCount} is out of bounds. It must be between {MIN_PLANETS} and {MAX_PLANETS}."
            );
            return;
        }
        numberOfPlanets = newPlanetCount;
        planetCountText.Text = numberOfPlanets.ToString();
    }

    private void OnStarTypeChanged(long index)
    {
        starTypeControl.Selected = (int)index;
        starType = (StarType)starTypeControl.Selected;
    }

    private void OnStartingResourcesMultiplierChanged(long index)
    {
        startingResourcesMultiplier = (StartingResourcesMultiplier)
            startingResourcesMultiplierControl.Selected;
    }

    private async void OnStartNewGame()
    {
        PlayerName = playerNameControl.Text;
        numberOfPlanets = planetCountControl.Value;
        starType = (StarType)starTypeControl.Selected;
        startingResourcesMultiplier = (StartingResourcesMultiplier)
            startingResourcesMultiplierControl.Selected;

        GD.Print(
            $"Starting new game with {numberOfPlanets} planets, star type {starType}, starting resources: energy {StartingEnergyResources} | mineral {StartingMineralResources}, player name {PlayerName}"
        );

        StartingResearch startingResearch = new();
        StartingResources startingResources =
            new() { energy = StartingEnergyResources, minerals = StartingMineralResources };

        GameSettings settings =
            new()
            {
                NumberOfPlanets = (int)numberOfPlanets,
                StarType = (int)starType,
                StartingResources = startingResources,
                StartingResearch = startingResearch,
                PlayerName = PlayerName
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

            GD.Print("Change scene to main game");
            // Use CallDeferred to ensure scene change happens on main thread
            CallDeferred(MethodName.ChangeToGameScene);
        }
        catch (Exception ex)
        {
            ErrorHandler.LogError("Error creating new game", ex, ErrorSeverity.Error);
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
