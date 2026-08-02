using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AI.IntegrationTests.Helpers;

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
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = TestAuthHandlerOptions.DefaultScheme;
            options.DefaultAuthenticateScheme = TestAuthHandlerOptions.DefaultScheme;
            options.DefaultChallengeScheme = TestAuthHandlerOptions.DefaultScheme;
        })
            .AddScheme<TestAuthHandlerOptions, TestAuthHandler>(
                TestAuthHandlerOptions.DefaultScheme, options => { });
    }

    public static void SetTestClaims(this HttpContext httpContext, IEnumerable<Claim> claims)
    {
        httpContext.Items["TestClaims"] = claims;
    }

    public static Claim[] CreateClaimsForUser(Guid userId, string email, string fullName, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Email, email),
            new("full_name", fullName)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return claims.ToArray();
    }

    public static string EncodeClaims(IEnumerable<Claim> claims)
    {
        var claimData = claims.Select(c => new { c.Type, c.Value }).ToList();
        var json = JsonSerializer.Serialize(claimData);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    public static void SetTestClaimsHeader(HttpClient client, IEnumerable<Claim> claims)
    {
        if (client.DefaultRequestHeaders.Contains("X-Test-Claims"))
            client.DefaultRequestHeaders.Remove("X-Test-Claims");
        client.DefaultRequestHeaders.Add("X-Test-Claims", EncodeClaims(claims));
    }
}
