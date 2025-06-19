using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

// Assume these namespaces and classes exist in the main project
// using YourAppNamespace.Services;
// using YourAppNamespace.Data;
// using YourAppNamespace.Models;

namespace TaskServiceTests
{
    // Sample Task entity
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; }
    }

    // Sample DbContext for tasks
    public class TasksDbContext : DbContext
    {
        public TasksDbContext(DbContextOptions<TasksDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }
    }

    // Sample TaskService class to be tested
    public class TaskService
    {
        private readonly TasksDbContext _context;

        public TaskService(TasksDbContext context)
        {
            _context = context;
        }

        // Gets all tasks
        public async Task<List<TaskItem>> GetAllTasksAsync()
        {
            return await _context.Tasks.ToListAsync();
        }

        // Creates a new task
        public async Task<TaskItem> CreateTaskAsync(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        // Marks a task complete
        public async Task<bool> MarkCompleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return false;
            task.IsComplete = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public class TaskServiceUnitTests : IDisposable
    {
        private readonly TasksDbContext _context;
        private readonly TaskService _service;

        public TaskServiceUnitTests()
        {
            // Setup InMemory EF Core database for unit tests
            var options = new DbContextOptionsBuilder<TasksDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TasksDbContext(options);
            _context.Database.EnsureCreated();

            _service = new TaskService(_context);

            // Seed data
            _context.Tasks.AddRange(
                new TaskItem { Id = 1, Title = "Test Task 1", IsComplete = false },
                new TaskItem { Id = 2, Title = "Test Task 2", IsComplete = true }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            var tasks = await _service.GetAllTasksAsync();
            Assert.NotNull(tasks);
            Assert.Equal(2, tasks.Count);
        }

        [Fact]
        public async Task CreateTaskAsync_AddsTask()
        {
            var newTask = new TaskItem { Title = "New Task", IsComplete = false };
            var createdTask = await _service.CreateTaskAsync(newTask);

            Assert.NotNull(createdTask);
            Assert.True(createdTask.Id > 0);

            var tasks = await _service.GetAllTasksAsync();
            Assert.Equal(3, tasks.Count);
            Assert.Contains(tasks, t => t.Title == "New Task");
        }

        [Fact]
        public async Task MarkCompleteAsync_MarksTaskComplete()
        {
            var result = await _service.MarkCompleteAsync(1);
            Assert.True(result);

            var task = await _context.Tasks.FindAsync(1);
            Assert.True(task.IsComplete);
        }

        [Fact]
        public async Task MarkCompleteAsync_ReturnsFalseForMissingTask()
        {
            var result = await _service.MarkCompleteAsync(999);
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
