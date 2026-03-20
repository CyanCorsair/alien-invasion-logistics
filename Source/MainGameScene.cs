using System;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.Utilities;
using Godot;

namespace AlienInvasionLogistics.Source;

public partial class MainGameScene : Node
{
    public enum GameMode
    {
        Strat,
        Tac
    }

    private const string StratUiScenePath = "res://Scenes/GameScreen/StratMapUI.tscn";
    private const string TacUiScenePath = "res://Scenes/GameScreen/TacMapUI.tscn";
    private const string StratWorldScenePath = "res://Scenes/Components/SolarSystemBase.tscn";
    private const string TacWorldScenePath = "res://Scenes/Components/TacWorldBase.tscn";

    private GameEventBus  _eventBus;
    
    private Control _activeUi;
    private Node2D _activeWorld;
    public Guid CurrentGameDataId { get; set; } = Guid.Empty;
    public Guid NextGameDataId { get; set; } = Guid.Empty;

    public GameMode CurrentMode { get; private set; }

    public override void _Ready()
    {
        _eventBus.Subscribe<GameCreatedEvent>(@event =>
        {
            if (CurrentGameDataId == Guid.Empty && NextGameDataId == Guid.Empty)
            {
                CurrentGameDataId = @event.GameDataId;
            }

            if (@event.GameDataId != CurrentGameDataId)
            {
                NextGameDataId = @event.GameDataId;
            }
        });
        
        try
        {
            GD.Print("MainGameScene ready.");
            SwapGameMode(null);
        }
        catch (Exception ex)
        {
            ErrorHandler.LogMessage("Error in MainGameScene _Ready", ex, ErrorUtilities.MessageLevel.Critical);
        }
    }

    public void SwapGameMode(GameMode? newMode)
    {
        try
        {
            if (newMode == CurrentMode)
                return;

            if (newMode == null)
            {
                CurrentMode = GameMode.Strat;
                LoadStratMode();
                return;
            }

            if (newMode == GameMode.Strat)
                LoadStratMode();
            else
                LoadTacMode();
        }
        catch (Exception ex)
        {
            ErrorHandler.LogMessage(
                $"Error swapping game mode to {newMode}",
                ex
            );
        }
    }

    private void LoadStratMode()
    {
        try
        {
            // Clean up existing scenes
            CleanupActiveScenes();

            // Load and instance the Strat UI
            var stratUiScene = GD.Load<PackedScene>(StratUiScenePath);
            if (stratUiScene == null)
                throw new InvalidOperationException($"Failed to load scene: {StratUiScenePath}");
            _activeUi = stratUiScene.Instantiate<Control>();
            AddChild(_activeUi);

            // Load and instance the Strat World
            var stratWorldScene = GD.Load<PackedScene>(StratWorldScenePath);
            if (stratWorldScene == null)
                throw new InvalidOperationException(
                    $"Failed to load scene: {StratWorldScenePath}"
                );
            _activeWorld = stratWorldScene.Instantiate<Node2D>();
            AddChild(_activeWorld);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleSceneLoadError(ex, "Strategic Mode");
        }
    }

    private void LoadTacMode()
    {
        try
        {
            // Clean up existing scenes
            CleanupActiveScenes();

            // Load and instance the Tac UI
            var tacUiScene = GD.Load<PackedScene>(TacUiScenePath);
            if (tacUiScene == null) throw new InvalidOperationException($"Failed to load scene: {TacUiScenePath}");
            _activeUi = tacUiScene.Instantiate<Control>();
            AddChild(_activeUi);

            // Load and instance the Tac World
            var tacWorldScene = GD.Load<PackedScene>(TacWorldScenePath);
            if (tacWorldScene == null)
                throw new InvalidOperationException(
                    $"Failed to load scene: {TacWorldScenePath}"
                );
            _activeWorld = tacWorldScene.Instantiate<Node2D>();
            AddChild(_activeWorld);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleSceneLoadError(ex, "Tactical Mode");
        }
    }

    private void CleanupActiveScenes()
    {
        if (_activeUi != null && IsInstanceValid(_activeUi))
        {
            _activeUi.QueueFree();
            _activeUi = null;
        }

        if (_activeWorld != null && IsInstanceValid(_activeWorld))
        {
            _activeWorld.QueueFree();
            _activeWorld = null;
        }
    }

    public override void _ExitTree()
    {
        CleanupActiveScenes();
    }
}