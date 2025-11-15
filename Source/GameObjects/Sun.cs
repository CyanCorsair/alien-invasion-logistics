using Core.Interfaces;
using Godot;
using System;

namespace Core.GameObjects
{
    public partial class Sun : Node2D, IStellarBody
    {
        private string _displayName = "Sun";
        private string _systemName = "Solar System";
        private float _mass = 1.989e30f; // Solar mass in kg (scaled down for game)

        [Export]
        public string DisplayName
        {
            get => _displayName;
            set => _displayName = value;
        }

        [Export]
        public string SystemName
        {
            get => _systemName;
            set => _systemName = value;
        }

        public Vector2 Location2D
        {
            get => Position;
            set => Position = value;
        }

        public Vector2 Velocity2D { get; set; } = Vector2.Zero;

        [Export]
        public float Mass
        {
            get => _mass;
            set => _mass = value;
        }

        public override void _Ready()
        {
            // Sun typically stays at the center of the solar system
            GD.Print($"Sun '{DisplayName}' initialized in system '{SystemName}'");
        }

        public override void _Process(double delta)
        {
            // Sun is generally stationary, but we keep the interface for consistency
            // Future: Could add solar rotation, pulsing effects, etc.
        }
    }
}
