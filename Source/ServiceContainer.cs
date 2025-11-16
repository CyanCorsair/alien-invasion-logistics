using Godot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Core.Database;
using Core.Database.Contexts;
using Core.Events;
using System;

namespace Core
{
    /// <summary>
    /// Godot autoload singleton that manages dependency injection
    /// </summary>
    [GlobalClass]
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
            var folder = System.Environment.SpecialFolder.LocalApplicationData;
            var path = System.Environment.GetFolderPath(folder);
            var dbPath = System.IO.Path.Join(path, "GameData.db");

            _services.AddDbContextFactory<GameDataContext>(
                options => options.UseSqlite($"Data Source={dbPath}")
            );

            // Register custom factory wrapper
            _services.AddSingleton<IGameDataContextFactory, GameDataContextFactory>(sp =>
            {
                var options = new DbContextOptionsBuilder<GameDataContext>()
                    .UseSqlite($"Data Source={dbPath}")
                    .Options;
                return new GameDataContextFactory(options);
            });

            // Register EventBus
            _services.AddSingleton<IEventBus>(sp =>
            {
                var eventBus = GetNode<EventBus>("/root/EventBus");

                if (eventBus is null)
                {
                    throw new InvalidOperationException(
                        "EventBus node not found in the scene tree."
                    );
                }
                return eventBus;
            });

            // Register services
            _services.AddSingleton<IGameDataService>(sp =>
            {
                var gameDataService = GetNode<GameDataService>("/root/GameDataService");
                var factory = sp.GetRequiredService<IGameDataContextFactory>();
                gameDataService.Initialize(factory);

                // Ensure database is created
                using var context = factory.CreateDbContext();
                context.Database.EnsureCreated();

                return gameDataService;
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
            if (_serviceProvider == null)
            {
                throw new InvalidOperationException("ServiceContainer not initialized!");
            }

            return _serviceProvider.GetRequiredService<T>();
        }

        public override void _ExitTree()
        {
            _serviceProvider?.Dispose();
        }
    }
}
