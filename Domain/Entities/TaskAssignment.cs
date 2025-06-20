using System;

namespace Domain.Entities
{
    // Represents the assignment of a user to a task
    public class TaskAssignment
    {
        // Identifier of the assigned task
        public Guid TaskId { get; set; }

        // Identifier of the assigned user
        public Guid UserId { get; set; }
    }
}
