using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Database;
using Core.Database.Models;
using Core.GameObjects;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    /// <summary>
    /// Manages instantiation and saving of solar system scenes from/to database
    /// </summary>
    public partial class SolarSystemSceneManager : Node
    {
        private readonly SolarSystemGenerator _generator;
        private IGameDataContextFactory _contextFactory;

        // References to instantiated nodes
        private Sun _sunNode;
        private List<Planet> _planetNodes = new();
        private Node2D _solarSystemContainer;

        public SolarSystemSceneManager(IGameDataContextFactory contextFactory = null)
        {
            _generator = new SolarSystemGenerator();
            _contextFactory = contextFactory;
        }

        public override void _ExitTree()
        {
            ClearSolarSystem();
            base._ExitTree();
        }

        /// <summary>
        /// Load and instantiate solar system from database
        /// </summary>
        /// <param name="solarSystemStateId">The unique ID of the solar system state to load</param>
        /// <param name="container">The Node2D container to add solar system nodes to</param>
        /// <returns>The container Node2D with instantiated solar system nodes</returns>
        /// <exception cref="InvalidOperationException">Thrown when context factory is not initialized</exception>
        /// <exception cref="KeyNotFoundException">Thrown when solar system state is not found in database</exception>
        public async Task<Node2D> LoadAndInstantiateSolarSystemAsync(
            Guid solarSystemStateId,
            Node2D container
        )
        {
            if (_contextFactory == null)
            {
                string message = "ContextFactory not initialized!";
                GD.PrintErr(message);
                throw new InvalidOperationException(message);
            }

            _solarSystemContainer = container;

            await using var context = _contextFactory.CreateDbContext();

            // Load solar system state with all related data
            var solarSystemState = await context.SolarSystemState
                .Include(s => s.SystemSun)
                .Include(s => s.PlanetarySystems)
                .ThenInclude(ps => ps.Planets)
                .FirstOrDefaultAsync(s => s.Id == solarSystemStateId);

            if (solarSystemState == null)
            {
                string message = $"Solar system state {solarSystemStateId} not found!";
                GD.PrintErr(message);
                throw new KeyNotFoundException(message);
            }

            // Clear existing nodes
            ClearSolarSystem();

            // Instantiate Sun
            if (solarSystemState.SystemSun != null)
            {
                _sunNode = _generator.InstantiateSunFromState(solarSystemState.SystemSun);
                container.AddChild(_sunNode);
                GD.Print($"Instantiated sun: {_sunNode.DisplayName}");
            }

            Vector2 sunPosition = _sunNode?.Position ?? Vector2.Zero;

            // Instantiate Planets
            foreach (var planetarySystem in solarSystemState.PlanetarySystems)
            {
                foreach (var planetState in planetarySystem.Planets)
                {
                    var planet = _generator.InstantiatePlanetFromState(planetState, sunPosition);
                    container.AddChild(planet);
                    _planetNodes.Add(planet);
                    GD.Print(
                        $"Instantiated planet: {planet.DisplayName} at radius {planet.OrbitalRadius}"
                    );
                }
            }

            GD.Print($"Solar system loaded: {_planetNodes.Count} planets");
            return container;
        }

        /// <summary>
        /// Save current solar system scene state back to database
        /// </summary>
        /// <param name="solarSystemStateId">The unique ID of the solar system state to update</param>
        /// <exception cref="InvalidOperationException">Thrown when context factory is not initialized</exception>
        public async Task SaveSolarSystemStateAsync(Guid solarSystemStateId)
        {
            if (_contextFactory == null)
            {
                GD.PrintErr("ContextFactory not initialized!");
                return;
            }

            await using var context = _contextFactory.CreateDbContext();

            // Load existing state
            var solarSystemState = await context.SolarSystemState
                .Include(s => s.SystemSun)
                .Include(s => s.PlanetarySystems)
                .ThenInclude(ps => ps.Planets)
                .FirstOrDefaultAsync(s => s.Id == solarSystemStateId);

            if (solarSystemState == null)
            {
                GD.PrintErr($"Solar system state {solarSystemStateId} not found!");
                return;
            }

            // Update sun state
            if (_sunNode != null && solarSystemState.SystemSun != null)
            {
                _generator.SaveSunState(_sunNode, solarSystemState.SystemSun);
            }

            // Update planet states
            int planetIndex = 0;
            foreach (var planetarySystem in solarSystemState.PlanetarySystems)
            {
                for (
                    int i = 0;
                    i < planetarySystem.Planets.Count && planetIndex < _planetNodes.Count;
                    i++, planetIndex++
                )
                {
                    _generator.SavePlanetState(
                        _planetNodes[planetIndex],
                        planetarySystem.Planets[i]
                    );
                }
            }

            await context.SaveChangesAsync();
            GD.Print("Solar system state saved to database");
        }

        /// <summary>
        /// Generate and instantiate a new solar system based on settings
        /// </summary>
        /// <param name="settings">Game settings containing generation parameters</param>
        /// <param name="container">The Node2D container to add solar system nodes to</param>
        /// <returns>The generated solar system state model</returns>
        public async Task<SolarSystemState> GenerateAndInstantiateNewSolarSystemAsync(
            GameSettingsModel settings,
            Node2D container
        )
        {
            // Generate solar system state
            var solarSystemState = _generator.GenerateSolarSystemState(settings);

            // Save to database if context factory is available
            if (_contextFactory != null)
            {
                await using var context = _contextFactory.CreateDbContext();
                context.SolarSystemState.Add(solarSystemState);
                await context.SaveChangesAsync();
                GD.Print($"Generated solar system saved to database: {solarSystemState.Id}");
            }

            // Instantiate the scene
            await LoadAndInstantiateSolarSystemAsync(solarSystemState.Id, container);

            return solarSystemState;
        }

        /// <summary>
        /// Clear all instantiated solar system nodes
        /// </summary>
        public void ClearSolarSystem()
        {
            if (_sunNode != null)
            {
                _sunNode.QueueFree();
                _sunNode = null;
            }

            foreach (var planet in _planetNodes)
            {
                planet?.QueueFree();
            }
            _planetNodes.Clear();
        }

        /// <summary>
        /// Get reference to the sun node
        /// </summary>
        /// <returns>The Sun node, or null if not instantiated</returns>
        public Sun GetSun() => _sunNode;

        /// <summary>
        /// Get references to all planet nodes
        /// </summary>
        /// <returns>A new list containing references to all planet nodes</returns>
        public List<Planet> GetPlanets() => new List<Planet>(_planetNodes);

        /// <summary>
        /// Get a specific planet by index
        /// </summary>
        /// <param name="index">Zero-based index of the planet</param>
        /// <returns>The Planet node at the specified index, or null if index is out of range</returns>
        public Planet GetPlanet(int index)
        {
            if (index >= 0 && index < _planetNodes.Count)
            {
                return _planetNodes[index];
            }
            return null;
        }

        /// <summary>
        /// Get planet count
        /// </summary>
        /// <returns>The number of instantiated planets</returns>
        public int GetPlanetCount() => _planetNodes.Count;
    }
}
