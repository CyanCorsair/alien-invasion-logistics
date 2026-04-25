using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Events;
using Godot;

namespace AlienInvasionLogistics.Source.Services;

public class GameObjectFactoryService
{
    // Scene paths
    private const string BaseSolarObjectPath = "res://Scenes/Components/SolarObjects/BaseSolarObject.tscn";

    // Scene cache for performance
    private readonly Dictionary<string, PackedScene> _sceneCache = new();
    private readonly IEventBus _eventBus;

    public GameObjectFactoryService(IEventBus eventBus)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        PreloadScenes();
    }
    
    private void PreloadScenes()
    {
        LoadAndCacheScene(BaseSolarObjectPath);
    }
    
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
