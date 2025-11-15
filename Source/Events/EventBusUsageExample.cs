using Godot;
using Core.Events;

namespace Core.Examples
{
    /// <summary>
    /// Example demonstrating how to use the EventBus system
    /// This file is for reference only and should not be instantiated in production
    /// </summary>
    public partial class EventBusUsageExample : Node
    {
        private IEventBus _eventBus;

        public override void _Ready()
        {
            // Get the EventBus from the ServiceContainer
            _eventBus = ServiceContainer.GetRequiredService<IEventBus>();

            // Subscribe to events
            SubscribeToEvents();

            // Example: Publish a GameCreated event
            PublishExampleEvents();
        }

        private void SubscribeToEvents()
        {
            // Subscribe to GameCreatedEvent
            _eventBus.Subscribe<GameCreatedEvent>(OnGameCreated);

            // Subscribe to ResourcesChangedEvent
            _eventBus.Subscribe<ResourcesChangedEvent>(OnResourcesChanged);

            // Subscribe to DayAdvancedEvent
            _eventBus.Subscribe<DayAdvancedEvent>(OnDayAdvanced);

            // Subscribe to ErrorOccurredEvent
            _eventBus.Subscribe<ErrorOccurredEvent>(OnErrorOccurred);
        }

        private void PublishExampleEvents()
        {
            // Example 1: Publish a GameCreatedEvent
            var gameCreatedEvent = new GameCreatedEvent(System.Guid.NewGuid(), "PlayerName");
            _eventBus.Publish(gameCreatedEvent);

            // Example 2: Publish a ResourcesChangedEvent
            var resourcesEvent = new ResourcesChangedEvent(
                energyStored: 1000,
                mineralsStored: 500,
                energyIncome: 10.5f,
                mineralsIncome: 5.2f
            );
            _eventBus.Publish(resourcesEvent);

            // Example 3: Publish a DayAdvancedEvent
            var dayEvent = new DayAdvancedEvent(currentDay: 42);
            _eventBus.Publish(dayEvent);
        }

        // Event Handlers
        private void OnGameCreated(GameCreatedEvent evt)
        {
            GD.Print($"Game created! Player: {evt.PlayerName}, GameID: {evt.GameDataId}");
            // Update UI, load initial game state, etc.
        }

        private void OnResourcesChanged(ResourcesChangedEvent evt)
        {
            GD.Print(
                $"Resources changed! Energy: {evt.EnergyStored}, Minerals: {evt.MineralsStored}"
            );
            // Update resource display in UI
        }

        private void OnDayAdvanced(DayAdvancedEvent evt)
        {
            GD.Print($"Day advanced to {evt.CurrentDay}");
            // Update day counter in UI, process daily events
        }

        private void OnErrorOccurred(ErrorOccurredEvent evt)
        {
            GD.PrintErr($"Error occurred: {evt.Message} (Severity: {evt.Severity})");
            // Show error dialog, log error, etc.
        }

        public override void _ExitTree()
        {
            // Unsubscribe from events when the node is removed
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<GameCreatedEvent>(OnGameCreated);
                _eventBus.Unsubscribe<ResourcesChangedEvent>(OnResourcesChanged);
                _eventBus.Unsubscribe<DayAdvancedEvent>(OnDayAdvanced);
                _eventBus.Unsubscribe<ErrorOccurredEvent>(OnErrorOccurred);
            }
        }
    }

    // ==================== USAGE PATTERNS ====================

    /*
     * PATTERN 1: Publishing events from a service
     *
     * public class GameDataService
     * {
     *     private IEventBus _eventBus;
     *
     *     public async Task CreateNewGameAsync(GameSettings settings)
     *     {
     *         // ... create game logic ...
     *
     *         // Publish event when done
     *         _eventBus.Publish(new GameCreatedEvent(gameDataId, settings.PlayerName));
     *     }
     * }
     */

    /*
     * PATTERN 2: Subscribing in UI controllers
     *
     * public partial class ResourceDisplayController : Control
     * {
     *     private IEventBus _eventBus;
     *
     *     public override void _Ready()
     *     {
     *         _eventBus = ServiceContainer.GetRequiredService<IEventBus>();
     *         _eventBus.Subscribe<ResourcesChangedEvent>(UpdateResourceDisplay);
     *     }
     *
     *     private void UpdateResourceDisplay(ResourcesChangedEvent evt)
     *     {
     *         energyLabel.Text = evt.EnergyStored.ToString();
     *         mineralsLabel.Text = evt.MineralsStored.ToString();
     *     }
     *
     *     public override void _ExitTree()
     *     {
     *         _eventBus?.Unsubscribe<ResourcesChangedEvent>(UpdateResourceDisplay);
     *     }
     * }
     */

    /*
     * PATTERN 3: Creating custom events
     *
     * // Define your event
     * public class BuildingConstructedEvent : GameEvent
     * {
     *     public string BuildingName { get; }
     *     public Vector2 Position { get; }
     *
     *     public BuildingConstructedEvent(string buildingName, Vector2 position)
     *     {
     *         BuildingName = buildingName;
     *         Position = position;
     *     }
     * }
     *
     * // Publish it
     * _eventBus.Publish(new BuildingConstructedEvent("Solar Panel", new Vector2(100, 200)));
     *
     * // Subscribe to it
     * _eventBus.Subscribe<BuildingConstructedEvent>(OnBuildingConstructed);
     */
}
