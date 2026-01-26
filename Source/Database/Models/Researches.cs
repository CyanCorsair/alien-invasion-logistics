using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AlienInvasionLogistics.Source.Database.Models;

[Owned]
public class ResearchItem
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Research Name";
    [Required][MaxLength(256)] public string Description { get; set; } = "Research Description";
    [Required] public ResearchStates ResearchState { get; set; } = ResearchStates.None;
    [MaxLength(256)] public string IconPath { get; set; } = "";
}

public enum ResearchStates
{
    None,
    Queued,
    InProgress,
    Completed
}
