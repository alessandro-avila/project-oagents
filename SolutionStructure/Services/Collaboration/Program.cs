using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register collaboration service dependencies here
builder.Services.AddScoped<ICollaborationService, CollaborationService>();

// Add controllers with API routing
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();


// Collaboration service interface defining business logic contract
public interface ICollaborationService
{
    IEnumerable<string> GetCollaborationRooms();
}

// Collaboration service implementation providing collaboration room data
public class CollaborationService : ICollaborationService
{
    public IEnumerable<string> GetCollaborationRooms() =>
        new List<string> { "Room1", "Room2", "Room3" };
}

// Sample API controller for collaboration
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CollaborationController : ControllerBase
{
    private readonly ICollaborationService _collaborationService;

    public CollaborationController(ICollaborationService collaborationService)
    {
        _collaborationService = collaborationService;
    }

    [HttpGet]
    public IEnumerable<string> Get() => _collaborationService.GetCollaborationRooms();
}
