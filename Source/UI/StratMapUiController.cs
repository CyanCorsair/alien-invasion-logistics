using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.GameObjects;
using AlienInvasionLogistics.Source.Interfaces;
using AlienInvasionLogistics.Source.Services;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.Utilities;
using Godot;
using Newtonsoft.Json;

namespace AlienInvasionLogistics.Source.UI;

public partial class StratMapUiController : Control
{
    private Guid _currentGameId;
    private StrategicWorldState _currentGameState;
    private GameDataService _gameDataService;
    private GameEventBus _gameEventBus;

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

    public override async void _Ready()
    {
        try
        {
            GD.Print("StratMapUiController ready.");
            await PrepareStratMap();
        }
        catch (Exception e)
        {
            GD.PrintErr(e);
        }
    }

    public override void _Process(double delta)
    {
    }

    public async Task PrepareStratMap()
    {
        await LoadGameData();
        PrepareSolarSystemScene();
    }

    private async Task SaveStrategicGameState()
    {
        await _gameDataService.SaveStrategicGameState(_currentGameId, _currentGameState);
    }

    private async Task LoadGameData()
    {
        _currentGameState = await _gameDataService.LoadStrategicGameState(_currentGameId);
    }

    private void PrepareSolarSystemScene()
    {
        // Set up central body Node2D
        BaseSolarObject centralMass = BaseSolarObject.CreateFromData(_currentGameState.SolarSystem.CentralMass);
        
        // Spawn major bodies via PlanetarySystem central mass
        // -> Spawn minor orbiting bodies in system
        
        
        // Spawn remaining bodies
        
        // Set up nation nodes
        
        // Set up player nodes
        
        // Spawn assets belonging to players
    }
}