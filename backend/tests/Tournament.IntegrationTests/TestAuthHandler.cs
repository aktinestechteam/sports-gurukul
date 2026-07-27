using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tournament.IntegrationTests;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "Test";
}

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = Context.Items["TestClaims"] as IEnumerable<Claim>;

        if (claims is null)
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity(claims, TestAuthHandlerOptions.DefaultScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestAuthHandlerOptions.DefaultScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class TestAuthExtensions
{
    public static void AddTestAuth(this IServiceCollection services)
    {
        services.AddAuthentication(TestAuthHandlerOptions.DefaultScheme)
            .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                TestAuthHandlerOptions.DefaultScheme, options => { });
    }

    public static void SetTestClaims(this HttpContext httpContext, IEnumerable<Claim> claims)
    {
        httpContext.Items["TestClaims"] = claims;
    }

    public static IEnumerable<Claim> CreateClaimsForRole(Guid userId, string email, string fullName, string role)
    {
        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("full_name", fullName),
            new Claim(ClaimTypes.Role, role)
        };
    }
}