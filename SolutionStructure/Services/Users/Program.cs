using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register user service dependencies here
builder.Services.AddScoped<IUserService, UserService>();

// Add controllers with API routing
builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();


// User service interface defining business logic contract
public interface IUserService
{
    IEnumerable<string> GetUsers();
}

// User service implementation providing user data
public class UserService : IUserService
{
    public IEnumerable<string> GetUsers() =>
        new List<string> { "User1", "User2", "User3" };
}

// Sample API controller for users
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public IEnumerable<string> Get() => _userService.GetUsers();
}
