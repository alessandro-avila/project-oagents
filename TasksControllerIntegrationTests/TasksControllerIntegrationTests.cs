using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;

// Assume these namespaces correspond to the main application
// using YourAppNamespace;
// using YourAppNamespace.Data;
// using YourAppNamespace.Models;

namespace TasksControllerIntegrationTests
{
    // Minimal sample TaskItem DTO matching API contract
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsComplete { get; set; }
    }

    // Minimal sample Startup class override for testing with InMemory DB
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<TasksDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                // Add DbContext using InMemory database for testing
                services.AddDbContext<TasksDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTasksTestDb");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<TasksDbContext>();

                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    // Seed initial data
                    db.Tasks.AddRange(
                        new TaskItem { Id = 1, Title = "Integration Task 1", IsComplete = false },
                        new TaskItem { Id = 2, Title = "Integration Task 2", IsComplete = true }
                    );
                    db.SaveChanges();
                }
            });
        }
    }

    public class TasksControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public TasksControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetTasks_ReturnsInitialTasks()
        {
            var response = await _client.GetAsync("/api/tasks");
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskItemDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(tasks);
            Assert.True(tasks.Count >= 2);
            Assert.Contains(tasks, t => t.Title == "Integration Task 1");
        }

        [Fact]
        public async Task PostTask_CreatesNewTask()
        {
            var newTask = new TaskItemDto { Title = "Integration New Task", IsComplete = false };
            var jsonContent = new StringContent(JsonSerializer.Serialize(newTask), Encoding.UTF8, "application/json");

            var postResponse = await _client.PostAsync("/api/tasks", jsonContent);
            postResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync("/api/tasks");
            getResponse.EnsureSuccessStatusCode();

            var responseString = await getResponse.Content.ReadAsStringAsync();
            var tasks = JsonSerializer.Deserialize<List<TaskItemDto>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.Contains(tasks, t => t.Title == "Integration New Task");
        }

        [Fact]
        public async Task PutMarkComplete_MarksTaskComplete()
        {
            // Mark task with Id=1 complete
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/tasks/1/complete");
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            // Verify the task is marked complete
            var getResponse = await _client.GetAsync("/api/tasks/1");
            getResponse.EnsureSuccessStatusCode();

            var responseString = await getResponse.Content.ReadAsStringAsync();
            var task = JsonSerializer.Deserialize<TaskItemDto>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.True(task.IsComplete);
        }

        [Fact]
        public async Task PutMarkComplete_ReturnsNotFoundForMissingTask()
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/tasks/999/complete");
            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
