using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register tasks service dependencies here
builder.Services.AddScoped<ITaskService, TaskService>();

// Add controllers with API routing
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();


// Task service interface defining business logic contract
public interface ITaskService
{
    IEnumerable<string> GetTasks();
}

// Task service implementation providing task data
public class TaskService : ITaskService
{
    public IEnumerable<string> GetTasks() =>
        new List<string> { "Task1", "Task2", "Task3" };
}

// Sample API controller for tasks
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public IEnumerable<string> Get() => _taskService.GetTasks();
}
