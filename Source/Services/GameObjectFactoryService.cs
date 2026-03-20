using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.GameObjects;
using Godot;

namespace AlienInvasionLogistics.Source.Services;

/// <summary>
/// Factory service for creating game objects from database models and DTOs.
/// </summary>
public class GameObjectFactoryService
{
    // Scene paths
    private const string BaseSolarObjectPath = "res://Scenes/Components/SolarObjects/BaseSolarObject.tscn";

    // Scene cache for performance
    private readonly Dictionary<string, PackedScene> _sceneCache = new();
    private IEventBus _eventBus;

    /// <summary>
    /// Initializes the factory service with required dependencies.
    /// </summary>
    public void Initialize(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        PreloadScenes();
    }

    public BaseSolarObject CreateSolarObjectFromData(BaseNaturalSolarObject baseNaturalSolarObjectData)
    {
        return BaseSolarObject.CreateFromData(baseNaturalSolarObjectData);
    }

    /// <summary>
    /// Validates that the service has been initialized.
    /// </summary>
    private void EnsureInitialized()
    {
        if (_eventBus == null)
            throw new InvalidOperationException("GameObjectFactoryService has not been initialized. Call Initialize() first.");
    }

    /// <summary>
    /// Preloads commonly used scenes into cache.
    /// </summary>
    private void PreloadScenes()
    {
        LoadAndCacheScene(BaseSolarObjectPath);
    }

    /// <summary>
    /// Loads and caches a PackedScene.
    /// </summary>
    private PackedScene LoadAndCacheScene(string path)
    {
        if (_sceneCache.TryGetValue(path, out var cached))
            return cached;

        var scene = GD.Load<PackedScene>(path);
        if (scene == null)
            throw new InvalidOperationException($"Failed to load scene: {path}");

        _sceneCache[path] = scene;
        return scene;
    }
}
