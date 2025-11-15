using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Database.Models
{
    public class PlayerStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Owned types - no separate table needed, stored inline
        [Required]
        public ResourcesState ResourcesState { get; set; } = new ResourcesState();
        [Required]
        public ResearchState ResearchState { get; set; } = new ResearchState();
    }

    public class AIPlayerStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Owned types - no separate table needed, stored inline
        [Required]
        public ResourcesState ResourcesState { get; set; } = new ResourcesState();
        [Required]
        public ResearchState ResearchState { get; set; } = new ResearchState();
    }
}
