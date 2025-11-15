using Core.Database.Models;
using Core.Interfaces;
using Core.Types.Research;
using Core.Types.Resource;
using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Core
{
    public partial class AiPlayer(string name, Guid id, StartingResearch startingResearch, StartingResources startingResources) : Node, IBasePlayer
    {
        public bool isHuman { get; set; } = false;
        public string PlayerName { get; set; } = name;
        public Guid PlayerId { get; set; } = id;
        public ResearchState Research { get; set; } = new()
        {
            CurrentResearch = null,
            ResearchQueue = new List<ResearchItem>(),
            CompletedResearch = startingResearch.startingResearch,
        };
        public ResourcesState Resources { get; set; } = new()
        {
            EnergyStored = startingResources.energy,
            MineralsStored = startingResources.minerals,
            EnergyIncomeDaily = 0,
            MineralsIncomeDaily = 0
        };
    }
}
