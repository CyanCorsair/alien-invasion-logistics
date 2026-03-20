using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Models;

[Owned]
public class GameResource
{
    [Required][MaxLength(128)] public string Name { get; set; } = "Resource Name";
    [Required]
    public ResourceTypes ResourceType { get; set; } = ResourceTypes.Minerals;
    [Required]
    public int Quantity { get; set; } = 0;
    [Required]
    public int MaxQuantity { get; set; } = int.MaxValue;
    public int DailyProduction { get; set; } = 0;
}