using Core.Interfaces;
using Godot;
using System;

namespace Core.GameObjects
{
    public partial class Planet : Node2D, IStellarBody
    {
        private string _displayName = "Planet";
        private string _systemName = "Solar System";
        private float _mass = 5.972e24f; // Earth mass in kg (scaled down for game)
        private Vector2 _velocity = Vector2.Zero;

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

        public Vector2 Velocity2D
        {
            get => _velocity;
            set => _velocity = value;
        }

        [Export]
        public float Mass
        {
            get => _mass;
            set => _mass = value;
        }

        [Export]
        public float OrbitalRadius { get; set; } = 100f;

        [Export]
        public float OrbitalSpeed { get; set; } = 1f;

        private float _orbitalAngle = 0f;
        private Vector2 _orbitCenter = Vector2.Zero;

        public override void _Ready()
        {
            GD.Print($"Planet '{DisplayName}' initialized in system '{SystemName}'");
            // Store the center point for orbital calculations
            _orbitCenter = Position - new Vector2(OrbitalRadius, 0);
        }

        public override void _Process(double delta)
        {
            // Simple orbital mechanics
            _orbitalAngle += OrbitalSpeed * (float)delta;

            // Calculate new position based on orbital radius and angle
            float x = _orbitCenter.X + OrbitalRadius * Mathf.Cos(_orbitalAngle);
            float y = _orbitCenter.Y + OrbitalRadius * Mathf.Sin(_orbitalAngle);

            Position = new Vector2(x, y);

            // Calculate velocity (tangent to orbit)
            _velocity = new Vector2(
                -OrbitalRadius * OrbitalSpeed * Mathf.Sin(_orbitalAngle),
                OrbitalRadius * OrbitalSpeed * Mathf.Cos(_orbitalAngle)
            );
        }

        public void SetOrbitCenter(Vector2 center)
        {
            _orbitCenter = center;
        }
    }
}
