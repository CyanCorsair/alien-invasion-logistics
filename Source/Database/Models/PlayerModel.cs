using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AlienInvasionLogistics.Source.Types;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

[PrimaryKey("Id")]
public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public bool IsHuman { get; set; } = false;

    [Required] [MaxLength(128)] public string Name { get; set; } = "New Player";

    [Required] public Guid GameSessionId { get; set; }

    [Required] public GameSession GameSession { get; set; }

    public Guid NationId { get; set; }

    public Nation Nation { get; set; }
    
    public ResearchState ResearchState { get; set; }

    public ResourcesState ResourceState { get; set; } = new();
}

[Owned]
public class ResearchState
{
    public List<ResearchItem> FinishedResearch { get; set; }
    public ResearchItem CurrentResearch { get; set; }
    public List<ResearchItem> ResearchQueue { get; set; }
    public List<ResearchItem> KnownResearch { get; set; }
}

[Owned]
public class ResourcesState
{
    public List<GameResource> Resources { get; set; }
}