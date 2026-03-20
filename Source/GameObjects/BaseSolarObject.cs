using System.Collections.Generic;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Types;
using AlienInvasionLogistics.Source.Utilities;
using Godot;

namespace AlienInvasionLogistics.Source.GameObjects;

public record StaticPhysicsProperties
{
    public float Mass { get; set; }
    public float Radius { get; set; }
    public float OrbitalPeriod { get; set; }
    public float SemiMajorAxis { get; set; }
    public float Eccentricity { get; set; }
}

public record StaticGameplayProperties
{
    public int SunlightLevel { get; set; }
}

public partial class BaseSolarObject : Node2D, IGameObject
{
    private Area2D InteractionArea { get; set; }
    private CollisionShape2D CollisionShape { get; set; }
    private Sprite2D SurfaceMap { get; set; }
    private Sprite2D Icon { get; set; }
    private StaticPhysicsProperties PhysicsProperties { get; set; }
    private StaticGameplayProperties GameplayProperties { get; set; }
    private Node2D ParentBody { get; set; }

    private float _currentTime;
    private float _initialAngle;
    private Vector2 _targetPosition;

    [Export] public float PositionX { get; set; }
    [Export] public float PositionY { get; set; }
    [Export] public float LerpSpeed { get; set; }

    public static BaseSolarObject CreateFromData(BaseNaturalSolarObject baseNaturalSolarObject)
    {
        BaseSolarObject baseSolarObject = new BaseSolarObject();
        baseSolarObject.PhysicsProperties = new StaticPhysicsProperties()
        {
            Mass = baseNaturalSolarObject.Mass,
            Radius = baseNaturalSolarObject.Radius,
            OrbitalPeriod = baseNaturalSolarObject.OrbitalPeriod,
            SemiMajorAxis = baseNaturalSolarObject.SemiMajorAxis,
            Eccentricity = baseNaturalSolarObject.Eccentricity,
        };

        baseSolarObject.GameplayProperties = new StaticGameplayProperties()
        {
            SunlightLevel = baseNaturalSolarObject.SunlightLevel,
        };

        return baseSolarObject;
    }
    
    private static float SolveKepler(float M, float e)                                                                                                                                                                                                                                                           
    {                                                                                                                                                                                                                                                                                                            
        if (e < 0.001f) return M;                                                                                                                                                                                                                                                                                
                                                                                                                                                                                                                                                                                                               
        float E = M;                                                                                                                                                                                                                                                                                             
        for (int i = 0; i < 6; i++)                                                                                                                                                                                                                                                                              
          E -= (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));                                                                                                                                                                                                                                            
                                                                                                                                                                                                                                                                                                               
        return 2 * Mathf.Atan2(                                                                                                                                                                                                                                                                                  
          Mathf.Sqrt(1 + e) * Mathf.Sin(E / 2),                                                                                                                                                                                                                                                                
          Mathf.Sqrt(1 - e) * Mathf.Cos(E / 2)                                                                                                                                                                                                                                                                 
        );                                                                                                                                                                                                                                                                                                       
    }

    public override void _Ready()
    {
        InteractionArea =  GetNode<Area2D>("InteractArea");
        CollisionShape = GetNode<CollisionShape2D>("InteractArea/CollisionShape");
        Icon = GetNode<Sprite2D>("Icon");
        
        SetInitialPosition();
    }

    public override void _Process(double delta)
    {
        GlobalPosition = GlobalPosition.Lerp(_targetPosition, (float)delta * LerpSpeed);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PhysicsProperties.OrbitalPeriod <= 0 || PhysicsProperties.SemiMajorAxis <= 0)
            return;

        _currentTime += (float)delta;
        Vector2 offset = CalculateOffset();
        SetTargetPosition(offset);
    }
    
    private void SetInitialPosition()
    {
        Vector2 offset = CalculateOffset();
        SetTargetPosition(offset);
        GlobalPosition = _targetPosition;
    }
    
    private void SetTargetPosition(Vector2 offset)
    {
        _targetPosition = ParentBody != null ? ParentBody.GlobalPosition + offset : offset;
    }

    private Vector2 CalculateOffset()
    {
        float meanAnomaly = (_initialAngle + Mathf.Tau * _currentTime / PhysicsProperties.OrbitalPeriod) % Mathf.Tau;
        float trueAnomaly = SolveKepler(meanAnomaly, PhysicsProperties.Eccentricity);
        
        float r = PhysicsProperties.SemiMajorAxis
                  * (1 - PhysicsProperties.Eccentricity * PhysicsProperties.Eccentricity)
                  / (1 + PhysicsProperties.Eccentricity * Mathf.Cos(trueAnomaly));
        
        return new Vector2(r * Mathf.Cos(trueAnomaly),  r * Mathf.Sin(trueAnomaly));
    }
}