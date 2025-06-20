using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    // Represents a task within a project management context
    public class Task
    {
        // Unique identifier for the task
        public Guid Id { get; set; }

        // Title of the task
        public string Title { get; set; }

        // Detailed description of the task
        public string Description { get; set; }

        // Due date for task completion
        public DateTime DueDate { get; set; }

        // Priority of the task
        public Priority Priority { get; set; }

        // Current status of the task
        public Status Status { get; set; }

        // Reference to the associated project
        public Guid ProjectId { get; set; }

        // List of user IDs assigned to this task
        public List<Guid> AssignedUserIds { get; set; } = new List<Guid>();
    }
}
