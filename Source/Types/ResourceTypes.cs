using AlienInvasionLogistics.Source.Interfaces;
using Godot;

namespace AlienInvasionLogistics.Source.Types;

public enum ResourceTypes
{
    Energy,
    Minerals
}

public class StartingResources
{
    public int Minerals;
    public int Energy;
}

public class Minerals : IGameResource
{
    public string Name { get; set; } = "Minerals";
    public Sprite2D Icon { get; set; }
}

public class Energy : IGameResource
{
    public string Name { get; set; } = "Energy";
    public Sprite2D Icon { get; set; }
}