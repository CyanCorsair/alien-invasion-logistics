using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Dtos;
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
    private const string STAR_SCENE_PATH = "res://Scenes/Components/SolarObjects/Sun.tscn";
    private const string PLANET_SCENE_PATH = "res://Scenes/Components/SolarObjects/Planet.tscn";

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
        LoadAndCacheScene(STAR_SCENE_PATH);
        LoadAndCacheScene(PLANET_SCENE_PATH);
    }

    /// <summary>
    /// Creates a star game object from a CelestialBody.
    /// </summary>
    public StarGameObject CreateStar(CelestialBody starData)
    {
        EnsureInitialized();
        var dto = starData.ToStarDto();
        return CreateFromDto(dto);
    }

    /// <summary>
    /// Creates a planet game object from a CelestialBody.
    /// </summary>
    public PlanetGameObject CreatePlanet(CelestialBody planetData)
    {
        EnsureInitialized();
        var dto = planetData.ToPlanetDto();
        return CreateFromDto(dto);
    }

    /// <summary>
    /// Creates a planetary system game object from a PlanetarySystem.
    /// </summary>
    public PlanetarySystemGameObject CreatePlanetarySystem(PlanetarySystem systemData)
    {
        EnsureInitialized();
        var dto = systemData.ToDto();
        return CreateFromDto(dto);
    }

    /// <summary>
    /// Creates a star game object from a DTO.
    /// </summary>
    public StarGameObject CreateFromDto(StarGameObjectDto dto)
    {
        EnsureInitialized();
        var scene = LoadAndCacheScene(STAR_SCENE_PATH);
        var star = scene.Instantiate<StarGameObject>();

        // Set properties
        star.Name = dto.Name;
        star.Position = new Vector2(dto.PositionX, dto.PositionY);

        // TODO: Add custom properties to StarGameObject class and populate them here
        // star.Mass = dto.Mass;
        // star.Radius = dto.Radius;
        // etc.

        return star;
    }

    /// <summary>
    /// Creates a planet game object from a DTO.
    /// </summary>
    public PlanetGameObject CreateFromDto(PlanetGameObjectDto dto)
    {
        EnsureInitialized();
        var scene = LoadAndCacheScene(PLANET_SCENE_PATH);
        var planet = scene.Instantiate<PlanetGameObject>();

        // Set properties
        planet.Name = dto.Name;
        planet.Position = new Vector2(dto.PositionX, dto.PositionY);

        // TODO: Add custom properties to PlanetGameObject class and populate them here
        // planet.Mass = dto.Mass;
        // planet.Radius = dto.Radius;
        // planet.OrbitalPeriod = dto.OrbitalPeriod;
        // etc.

        return planet;
    }

    /// <summary>
    /// Creates a planetary system game object from a DTO.
    /// </summary>
    public PlanetarySystemGameObject CreateFromDto(PlanetarySystemGameObjectDto dto)
    {
        EnsureInitialized();
        var system = new PlanetarySystemGameObject();
        system.Name = dto.Name;

        // TODO: Add custom properties to PlanetarySystemGameObject class
        // system.CentralMassId = dto.CentralMassId;
        // etc.

        return system;
    }

    /// <summary>
    /// Converts a game object to DTO for saving.
    /// </summary>
    public StarGameObjectDto ToDto(StarGameObject gameObject)
    {
        EnsureInitialized();
        return new StarGameObjectDto
        {
            // TODO: Extract properties from game object
            Name = gameObject.Name,
            PositionX = gameObject.Position.X,
            PositionY = gameObject.Position.Y
            // Add more properties as they're added to the game object
        };
    }

    /// <summary>
    /// Converts a planet game object to DTO for saving.
    /// </summary>
    public PlanetGameObjectDto ToDto(PlanetGameObject gameObject)
    {
        EnsureInitialized();
        return new PlanetGameObjectDto
        {
            Name = gameObject.Name,
            PositionX = gameObject.Position.X,
            PositionY = gameObject.Position.Y
            // Add more properties as they're added to the game object
        };
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
