using System;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.Interfaces;
using AlienInvasionLogistics.Source.Services;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.Utilities;
using Godot;
using Newtonsoft.Json;

namespace AlienInvasionLogistics.Source.UI;

public struct StrategicGameData
{
}

public partial class StratMapUiController : Control
{
    private Guid _currentGameId;
    private GameSession _currentGameSession;
    private StrategicGameData _currentGameData;
    private GameDataService _gameDataService;
    private GameEventBus _gameEventBus;
    private Node2D _solarSystemReference;

    public StratMapUiController()
    {
        GameDataService gameDataService = ServiceContainer.GetService<GameDataService>();
        GameEventBus gameEventBus = ServiceContainer.GetService<GameEventBus>();

        gameEventBus.Subscribe<GameCreatedEvent>(gameCreated =>
        {
            _currentGameId = gameCreated.GameDataId;
        });
        
        gameEventBus.Subscribe<GameLoadedEvent>(gameCreated =>
        {
            _currentGameId = gameCreated.GameDataId;
        });
    }

    public override void _Ready()
    {
        GD.Print("StratMapUiController ready.");
    }

    public override void _Process(double delta)
    {
    }

    public async Task PrepareStratMap()
    {
        await LoadGameData();
        PrepareSolarSystemScene();
        
    }

    private async Task SaveGameData()
    {
        await _gameDataService.SaveGameAsync(_currentGameSession);
    }

    private async Task LoadGameData()
    {
        _currentGameSession = await _gameDataService.LoadGameAsync(_currentGameId);
    }

    private void PrepareSolarSystemScene()
    {
        var solarSystemGenerator = ServiceContainer.GetService<SolarSystemGenerator>();
        StartingSettings settings = new StartingSettings()
        {
            GameName = _currentGameSession.SessionName,
            Difficulty = (GameDifficulty)_currentGameSession.DifficultyLevel
        };
        SolarSystem newSolarSystem = solarSystemGenerator.GenerateSolarSystemState(settings);
        _gameEventBus.Publish(
            new SolarSystemGeneratedEvent(newSolarSystem.Id, newSolarSystem.PlanetarySystems.Count));
        
        ErrorHandler.LogMessage($"{JsonConvert.SerializeObject(
            newSolarSystem, Formatting.Indented)}",
            severity: ErrorUtilities.MessageLevel.Info);
    }

    private void UpdateUiElements()
    {
    }
}