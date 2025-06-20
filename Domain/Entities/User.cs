using System;

namespace Domain.Entities
{
    // Represents a user who can be assigned to tasks
    public class User
    {
        // Unique identifier for the user
        public Guid Id { get; set; }

        // Name of the user
        public string Name { get; set; }

        // Email address of the user
        public string Email { get; set; }
    }
}
