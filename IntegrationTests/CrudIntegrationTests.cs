using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

// Define DTOs to match API contracts for serialization/deserialization
public record TaskDto(int Id, string Title, string Description);
public record CreateTaskDto(string Title, string Description);
public record UpdateTaskDto(string Title, string Description);

public record ProjectDto(int Id, string Name, string Description);
public record CreateProjectDto(string Name, string Description);
public record UpdateProjectDto(string Name, string Description);

public record UserDto(string Id, string Email);
public record CreateUserDto(string Email, string Password);
public record UpdateUserDto(string Email);

public class CrudIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CrudIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    #region Task CRUD tests

    [Fact]
    public async Task CreateTask_ShouldReturnCreatedTask()
    {
        var newTask = new CreateTaskDto("Test Task", "Task description");
        var response = await _client.PostAsJsonAsync("/api/tasks", newTask);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskDto>();
        createdTask.Should().NotBeNull();
        createdTask!.Title.Should().Be(newTask.Title);
        createdTask.Description.Should().Be(newTask.Description);
    }

    [Fact]
    public async Task GetTask_ShouldReturnTask()
    {
        // First create a task
        var newTask = new CreateTaskDto("Get Task", "Task to retrieve");
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetchedTask = await getResponse.Content.ReadFromJsonAsync<TaskDto>();
        fetchedTask.Should().NotBeNull();
        fetchedTask!.Title.Should().Be(newTask.Title);
    }

    [Fact]
    public async Task UpdateTask_ShouldModifyTask()
    {
        var newTask = new CreateTaskDto("Update Task", "Task to update");
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

        var updateDto = new UpdateTaskDto("Updated Task Title", "Updated description");
        var updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{createdTask!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        var updatedTask = await getResponse.Content.ReadFromJsonAsync<TaskDto>();
        updatedTask!.Title.Should().Be(updateDto.Title);
        updatedTask.Description.Should().Be(updateDto.Description);
    }

    [Fact]
    public async Task DeleteTask_ShouldRemoveTask()
    {
        var newTask = new CreateTaskDto("Delete Task", "Task to delete");
        var createResponse = await _client.PostAsJsonAsync("/api/tasks", newTask);
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{createdTask!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/tasks/{createdTask.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Project CRUD tests

    [Fact]
    public async Task CreateProject_ShouldReturnCreatedProject()
    {
        var newProject = new CreateProjectDto("Test Project", "Project description");
        var response = await _client.PostAsJsonAsync("/api/projects", newProject);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdProject = await response.Content.ReadFromJsonAsync<ProjectDto>();
        createdProject.Should().NotBeNull();
        createdProject!.Name.Should().Be(newProject.Name);
        createdProject.Description.Should().Be(newProject.Description);
    }

    [Fact]
    public async Task GetProject_ShouldReturnProject()
    {
        var newProject = new CreateProjectDto("Get Project", "Project to retrieve");
        var createResponse = await _client.PostAsJsonAsync("/api/projects", newProject);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        var getResponse = await _client.GetAsync($"/api/projects/{createdProject!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetchedProject = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        fetchedProject.Should().NotBeNull();
        fetchedProject!.Name.Should().Be(newProject.Name);
    }

    [Fact]
    public async Task UpdateProject_ShouldModifyProject()
    {
        var newProject = new CreateProjectDto("Update Project", "Project to update");
        var createResponse = await _client.PostAsJsonAsync("/api/projects", newProject);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        var updateDto = new UpdateProjectDto("Updated Project Name", "Updated project description");
        var updateResponse = await _client.PutAsJsonAsync($"/api/projects/{createdProject!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/projects/{createdProject.Id}");
        var updatedProject = await getResponse.Content.ReadFromJsonAsync<ProjectDto>();
        updatedProject!.Name.Should().Be(updateDto.Name);
        updatedProject.Description.Should().Be(updateDto.Description);
    }

    [Fact]
    public async Task DeleteProject_ShouldRemoveProject()
    {
        var newProject = new CreateProjectDto("Delete Project", "Project to delete");
        var createResponse = await _client.PostAsJsonAsync("/api/projects", newProject);
        var createdProject = await createResponse.Content.ReadFromJsonAsync<ProjectDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/projects/{createdProject!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/projects/{createdProject.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region User CRUD tests

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        var newUser = new CreateUserDto("newuser@example.com", "P@ssword123");
        var response = await _client.PostAsJsonAsync("/api/users", newUser);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdUser = await response.Content.ReadFromJsonAsync<UserDto>();
        createdUser.Should().NotBeNull();
        createdUser!.Email.Should().Be(newUser.Email);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser()
    {
        var newUser = new CreateUserDto("getuser@example.com", "P@ssword123");
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var getResponse = await _client.GetAsync($"/api/users/{createdUser!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetchedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        fetchedUser.Should().NotBeNull();
        fetchedUser!.Email.Should().Be(newUser.Email);
    }

    [Fact]
    public async Task UpdateUser_ShouldModifyUser()
    {
        var newUser = new CreateUserDto("updateuser@example.com", "P@ssword123");
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var updateDto = new UpdateUserDto("updateduser@example.com");
        var updateResponse = await _client.PutAsJsonAsync($"/api/users/{createdUser!.Id}", updateDto);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");
        var updatedUser = await getResponse.Content.ReadFromJsonAsync<UserDto>();
        updatedUser!.Email.Should().Be(updateDto.Email);
    }

    [Fact]
    public async Task DeleteUser_ShouldRemoveUser()
    {
        var newUser = new CreateUserDto("deleteuser@example.com", "P@ssword123");
        var createResponse = await _client.PostAsJsonAsync("/api/users", newUser);
        var createdUser = await createResponse.Content.ReadFromJsonAsync<UserDto>();

        var deleteResponse = await _client.DeleteAsync($"/api/users/{createdUser!.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getResponse = await _client.GetAsync($"/api/users/{createdUser.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region Azure AD B2C Authentication Tests

    [Fact]
    public async Task AccessProtectedEndpoint_ShouldReturnOkWithAuth()
    {
        // Example protected endpoint
        var response = await _client.GetAsync("/api/protectedresource");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AccessProtectedEndpoint_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Create client without auth header
        var unauthClient = new CustomWebApplicationFactory<Program>().CreateClient(new System.Net.Http.HttpClientHandler());
        var response = await unauthClient.GetAsync("/api/protectedresource");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}
