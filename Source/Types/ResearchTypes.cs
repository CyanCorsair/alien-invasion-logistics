using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Types.Research
{
    public class ResearchItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public struct StartingResearch
    {
        public List<ResearchItem> startingResearch;
    }
}
