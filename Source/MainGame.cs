using Godot;
using System;
using Core.Utilities;

namespace Core;

public partial class MainGame : Node
{
    Control activeUi;
    Node2D activeWorld;

    public enum GameMode
    {
        Strat,
        Tac
    }

    private GameMode _currentMode;
    public GameMode CurrentMode
    {
        get { return _currentMode; }
        private set { _currentMode = value; }
    }

    const string STRAT_UI_SCENE_PATH = "res://Scenes/GameScreen/StratMapUI.tscn";
    const string TAC_UI_SCENE_PATH = "res://Scenes/GameScreen/TacMapUI.tscn";
    const string STRAT_WORLD_SCENE_PATH = "res://Scenes/Components/SolarSystemBase.tscn";
    const string TAC_WORLD_SCENE_PATH = "res://Scenes/Components/TacWorldBase.tscn";

    public override void _Ready()
    {
        try
        {
            GD.Print("MainGame ready.");
            SwapGameMode(null);
        }
        catch (Exception ex)
        {
            ErrorHandler.LogError("Error in MainGame _Ready", ex, ErrorSeverity.Critical);
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
            {
                LoadStratMode();
            }
            else
            {
                LoadTacMode();
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.LogError(
                $"Error swapping game mode to {newMode}",
                ex,
                ErrorSeverity.Error
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
            PackedScene stratUiScene = GD.Load<PackedScene>(STRAT_UI_SCENE_PATH);
            if (stratUiScene == null)
            {
                throw new InvalidOperationException($"Failed to load scene: {STRAT_UI_SCENE_PATH}");
            }
            activeUi = stratUiScene.Instantiate<Control>();
            AddChild(activeUi);

            // Load and instance the Strat World
            PackedScene stratWorldScene = GD.Load<PackedScene>(STRAT_WORLD_SCENE_PATH);
            if (stratWorldScene == null)
            {
                throw new InvalidOperationException(
                    $"Failed to load scene: {STRAT_WORLD_SCENE_PATH}"
                );
            }
            activeWorld = stratWorldScene.Instantiate<Node2D>();
            AddChild(activeWorld);
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
            PackedScene tacUiScene = GD.Load<PackedScene>(TAC_UI_SCENE_PATH);
            if (tacUiScene == null)
            {
                throw new InvalidOperationException($"Failed to load scene: {TAC_UI_SCENE_PATH}");
            }
            activeUi = tacUiScene.Instantiate<Control>();
            AddChild(activeUi);

            // Load and instance the Tac World
            PackedScene tacWorldScene = GD.Load<PackedScene>(TAC_WORLD_SCENE_PATH);
            if (tacWorldScene == null)
            {
                throw new InvalidOperationException(
                    $"Failed to load scene: {TAC_WORLD_SCENE_PATH}"
                );
            }
            activeWorld = tacWorldScene.Instantiate<Node2D>();
            AddChild(activeWorld);
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleSceneLoadError(ex, "Tactical Mode");
        }
    }

    private void CleanupActiveScenes()
    {
        if (activeUi != null && IsInstanceValid(activeUi))
        {
            activeUi.QueueFree();
            activeUi = null;
        }

        if (activeWorld != null && IsInstanceValid(activeWorld))
        {
            activeWorld.QueueFree();
            activeWorld = null;
        }
    }

    public override void _ExitTree()
    {
        CleanupActiveScenes();
    }
}
