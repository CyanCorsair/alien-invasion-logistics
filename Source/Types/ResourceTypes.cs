using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Godot;

namespace Core.Types.Resource
{
    public enum RESOURCE_TYPES
    {
        ENERGY,
        MINERALS
    }

    public struct StartingResources
    {
        public int minerals;
        public int energy;
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
}
