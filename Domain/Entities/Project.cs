using System;

namespace Domain.Entities
{
    // Represents a project that groups tasks
    public class Project
    {
        // Unique identifier for the project
        public Guid Id { get; set; }

        // Name of the project
        public string Name { get; set; }

        // Description of the project
        public string Description { get; set; }
    }
}
