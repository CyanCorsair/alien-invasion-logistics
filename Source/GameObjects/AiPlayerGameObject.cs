using System;
using AlienInvasionLogistics.Source.Database.Models;
using AlienInvasionLogistics.Source.Interfaces;
using AlienInvasionLogistics.Source.Types;
using Godot;

namespace AlienInvasionLogistics.Source.GameObjects;

public partial class AiPlayerGameObject(
    string name,
    Guid id,
    StartingResearch startingResearch,
    StartingResources startingResources) : Node, IBasePlayer
{
    public bool IsHuman { get; set; } = false;
    public string PlayerName { get; set; } = name;
    public Guid PlayerId { get; set; } = id;

    public ResearchState Researches { get; set; } = new()
    {
        CurrentResearch = null,
        ResearchQueue = [],
        FinishedResearch = startingResearch.startingResearch,
        KnownResearch = []
    };

    public ResourcesState Resources { get; set; } = new()
    {
        Resources = []
    };
}