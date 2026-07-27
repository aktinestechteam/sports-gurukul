using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.IntegrationTests.Fixtures;
using SportsGurukul.IntegrationTests.Helpers;
using Xunit;

namespace SportsGurukul.IntegrationTests;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected CustomWebApplicationFactory Factory { get; }

    protected TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    protected HttpClient HttpClient =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    protected HttpClient CreateClientAsRole(string role)
    {
        var claims = CreateClaimsForRole(role);

        var client = Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("Test", _ => { });
                services.AddSingleton<IStartupFilter>(new TestClaimsStartupFilter());
            });
        }).CreateClient();

        SetClaimsForClient(client, claims);
        return client;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return Factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication("Test")
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("Test", _ => { });
                services.AddSingleton<IStartupFilter>(new TestClaimsStartupFilter());
            });
        }).CreateClient();
    }

    protected void SetClaimsForClient(HttpClient client, IEnumerable<Claim> claims)
    {
        var claimList = claims.ToList();

        if (client.DefaultRequestHeaders.Contains("X-Test-Claims"))
            client.DefaultRequestHeaders.Remove("X-Test-Claims");

        var claimData = claimList.Select(c => new { c.Type, c.Value }).ToList();
        var json = JsonSerializer.Serialize(claimData);
        client.DefaultRequestHeaders.Add("X-Test-Claims",
            Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));
    }

    protected void SetClaimsForClient(HttpClient client, Claim[] claims)
    {
        SetClaimsForClient(client, (IEnumerable<Claim>)claims);
    }

    protected async Task<HttpResponseMessage> PostJsonAsync<T>(HttpClient client, string url, T body)
    {
        return await client.PostAsJsonAsync(url, body);
    }

    protected async Task<HttpResponseMessage> PutJsonAsync<T>(HttpClient client, string url, T body)
    {
        return await client.PutAsJsonAsync(url, body);
    }

    protected async Task<HttpResponseMessage> GetAsync(HttpClient client, string url)
    {
        return await client.GetAsync(url);
    }

    protected async Task<HttpResponseMessage> DeleteAsync(HttpClient client, string url)
    {
        return await client.DeleteAsync(url);
    }

    protected async Task<HttpResponseMessage> PostAsync(HttpClient client, string url)
    {
        return await client.PostAsync(url, null);
    }

    private static Claim[] CreateClaimsForRole(string role)
    {
        var (userId, email, name) = role switch
        {
            "System Admin" => (TestIds.SystemAdminUserId, TestConstants.SystemAdminEmail, TestConstants.SystemAdminName),
            "Academy Admin" => (TestIds.AcademyAdminUserId, TestConstants.AcademyAdminEmail, TestConstants.AcademyAdminName),
            "Coach" => (TestIds.CoachUserId, TestConstants.CoachEmail, TestConstants.CoachName),
            "Athlete" => (TestIds.AthleteUserId, TestConstants.AthleteEmail, TestConstants.AthleteName),
            _ => throw new ArgumentException($"Unknown role: {role}", nameof(role))
        };

        return new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("full_name", name),
            new Claim(ClaimTypes.Role, role)
        };
    }

    private sealed class TestClaimsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.Use(async (context, requestNext) =>
                {
                    if (context.Request.Headers.TryGetValue("X-Test-Claims", out var header))
                    {
                        var bytes = Convert.FromBase64String(header!);
                        var json = Encoding.UTF8.GetString(bytes);
                        var claimData = JsonSerializer.Deserialize<List<ClaimData>>(json);
                        if (claimData != null)
                        {
                            context.Items["TestClaims"] = claimData.Select(c => new Claim(c.Type, c.Value));
                        }
                    }
                    await requestNext();
                });
                next(app);
            };
        }
    }

    private sealed class ClaimData
    {
        public string Type { get; set; } = "";
        public string Value { get; set; } = "";
    }
}