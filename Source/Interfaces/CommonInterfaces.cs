using Core.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Core.Interfaces
{
    public interface IGameResource
    {
        public string Name { get; set; }
        public Sprite2D Icon { get; set; }
    }

    public interface IBasePlayer
    {
        public bool isHuman { get; set; }
        public string PlayerName { get; set; }
        public Guid PlayerId { get; set; }
        public ResearchState Research { get; set; }
        public ResourcesState Resources { get; set; }
    }

    public interface IStellarBody
    {
        public string DisplayName { get; set; }
        public string SystemName { get; set; }
        public Vector2 Location2D { get; set; }
        public Vector2 Velocity2D { get; set; }
        public float Mass { get; set; }
    }

    public interface IPersistable
    {
        public abstract void Save();
        public abstract void Load();
    }
}
