using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using BackendApi; // Replace with actual backend API namespace

// Custom web application factory to configure Azure AD B2C auth for integration tests
public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureClient(HttpClient client)
    {
        // Add a default authenticated user header simulating Azure AD B2C token
        client.DefaultRequestHeaders.Add("Authorization", "Bearer fake-jwt-token-for-tests");
        base.ConfigureClient(client);
    }

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Replace authentication scheme with a test authentication handler
            services.RemoveAll(typeof(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            }).AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options => { });
        });
    }
}

// Test authentication handler to bypass Azure AD B2C during tests
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, 
        UrlEncoder encoder, ISystemClock clock) 
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Create a test authenticated user principal
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Name, "testuser@example.com"),
            new Claim("emails", "testuser@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestScheme");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "TestScheme");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
