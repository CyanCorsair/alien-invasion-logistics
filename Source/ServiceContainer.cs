using System;
using System.IO;
using AlienInvasionLogistics.Source.Database;
using AlienInvasionLogistics.Source.Database.Contexts;
using AlienInvasionLogistics.Source.Events;
using AlienInvasionLogistics.Source.Services;
using Godot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Environment = System.Environment;

namespace AlienInvasionLogistics.Source;

/// <summary>
///     Godot autoload singleton that manages dependency injection
/// </summary>
public partial class ServiceContainer : Node
{
    private static ServiceProvider _serviceProvider;
    private static IServiceCollection _services;

    public override void _Ready()
    {
        GD.Print("Initializing ServiceContainer...");
        ConfigureServices();
        GD.Print("ServiceContainer initialized");
    }

    private void ConfigureServices()
    {
        _services = new ServiceCollection();

        // Configure DbContext with factory pattern
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var dbPath = Path.Join(path, "GameData.db");

        _services.AddDbContextFactory<GameDataContext>(options => options.UseSqlite($"Data Source={dbPath}")
        );

        // Register custom factory wrapper
        _services.AddSingleton<IGameDataContextFactory, GameDataContextFactory>(sp =>
        {
            var options = new DbContextOptionsBuilder<GameDataContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            return new GameDataContextFactory(options);
        });

        // Register GameEventBus
        _services.AddSingleton<IEventBus>(sp =>
        {
            var eventBus = GetNode<GameEventBus>("/root/GameEventBus");

            if (eventBus is null)
                throw new InvalidOperationException(
                    "GameEventBus node not found in the scene tree."
                );
            return eventBus;
        });

        // Register services
        _services.AddSingleton<IGameDataService>(sp =>
        {
            var gameDataService = GetNode<GameDataService>("/root/GameDataService");
            var factory = sp.GetRequiredService<IGameDataContextFactory>();
            var eventBus = sp.GetRequiredService<IEventBus>();
            gameDataService.Initialize(factory, eventBus);

            // Ensure database is created
            using var context = factory.CreateDbContext();
            context.Database.EnsureCreated();

            return gameDataService;
        });

        _services.AddSingleton<ISolarSystemGenerator>(sp =>
        {
            var generator = GetNode<SolarSystemGenerator>("/root/SolarSystemGenerator");
            var eventBus = sp.GetRequiredService<IEventBus>();
            generator.Initialize(eventBus);
            return generator;
        });

        // Register GameObjectFactoryService (plain C# class)
        _services.AddSingleton<GameObjectFactoryService>(sp =>
        {
            var eventBus = sp.GetRequiredService<IEventBus>();
            return new GameObjectFactoryService(eventBus);
        });

        _serviceProvider = _services.BuildServiceProvider();
    }

    public static T GetService<T>()
    {
        if (_serviceProvider == null)
        {
            GD.PrintErr("ServiceContainer not initialized!");
            return default;
        }

        return _serviceProvider.GetService<T>();
    }

    public static T GetRequiredService<T>()
    {
        if (_serviceProvider == null) throw new InvalidOperationException("ServiceContainer not initialized!");

        return _serviceProvider.GetRequiredService<T>();
    }

    public override void _ExitTree()
    {
        _serviceProvider?.Dispose();
    }
}