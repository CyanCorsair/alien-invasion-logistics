using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using AlienInvasionLogistics.Source.Types;

namespace AlienInvasionLogistics.Source.Database.Models;

[Owned]
public class GameResource
{
    [Required]
    public ResourceTypes ResourceType { get; set; } = ResourceTypes.Minerals;
    public string Name => ResourceType.ToString();
    [Required]
    public int Quantity { get; set; } = 0;
    [Required]
    public int MaxQuantity { get; set; } = int.MaxValue;
    public int DailyProduction { get; set; } = 0;
}