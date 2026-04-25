using System;
using System.Collections.Generic;
using AlienInvasionLogistics.Source.Interfaces;
using AlienInvasionLogistics.Source.UI.MenuControllers;
using Godot;

namespace AlienInvasionLogistics.Source.Types;

public enum StarSize
{
    Dwarf,
    Medium,
    Giant,
    HyperGiant
}

public enum OrbitType
{
    LowOrbit,           // LEO - ~1.2x body radius
    GeostationaryOrbit, // GEO - orbital period matches rotation
    HighOrbit           // HEO - near Hill sphere limit
}

public class Star : IStellarBody
{
    public Color StarColor { get; set; }
    public StarSize StarSize { get; set; }
    public string DisplayName { get; set; }
    public Vector2 Location2D { get; set; }
    public Vector2 Velocity2D { get; set; }
    public float Mass { get; set; }

    public static Star CreateFromData(StarType starType)
    {
        Star newStar = new Star()
        {
            DisplayName = "New star",
            Location2D = new Vector2(0, 0),
            Velocity2D = new Vector2(0, 0)
        };
        
        switch (starType)
        {
            case StarType.MainSequence:
                newStar.Mass = 100;
                newStar.StarColor = new Color(255f, 255f, 255f);
                newStar.StarSize = StarSize.Medium;
                break;
            case StarType.BlueGiant:
                newStar.Mass = 90;
                newStar.StarColor = new Color(0f, 0f, 255f);
                newStar.StarSize = StarSize.Giant;
                break;
            case StarType.RedDwarf:
                newStar.Mass = 125;
                newStar.StarColor = new Color(255f, 0f, 0f);
                newStar.StarSize = StarSize.Dwarf;
                break;
        }

        return newStar;
    }
}