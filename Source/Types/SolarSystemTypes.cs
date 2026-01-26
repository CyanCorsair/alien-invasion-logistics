using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Interfaces;
using Godot;

namespace AlienInvasionLogistics.Source.Types;

public enum StarSize
{
    Dwarf,
    Medium,
    Giant,
    HyperGiant
}

public class Star : IStellarBody
{
    public Color StarColor { get; set; }
    public StarSize StarSize { get; set; }
    public string DisplayName { get; set; }
    public Vector2 Location2D { get; set; }
    public Vector2 Velocity2D { get; set; }
    public float Mass { get; set; }
}