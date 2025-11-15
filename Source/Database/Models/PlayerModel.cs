using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Database.Models
{
    public class PlayerStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Owned types - no separate table needed, stored inline
        public ResourcesState ResourcesState { get; set; }
        public ResearchState ResearchState { get; set; }
    }

    public class AIPlayerStateModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        // Owned types - no separate table needed, stored inline
        public ResourcesState ResourcesState { get; set; }
        public ResearchState ResearchState { get; set; }
    }
}
