using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Npgsql;

namespace Tournament.IntegrationTests;

public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly CustomWebApplicationFactory Factory;

    private static Respawner? _respawner;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
        await SeedDataAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    protected virtual async Task SeedDataAsync()
    {
    }

    protected async Task ResetDatabaseAsync()
    {
        var connectionString = Factory.ConnectionString;

        await _semaphore.WaitAsync();
        try
        {
            if (_respawner == null)
            {
                await using var conn = new NpgsqlConnection(connectionString);
                await conn.OpenAsync();
                _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    TablesToIgnore = ["__EFMigrationsHistory"]
                });
            }
        }
        finally
        {
            _semaphore.Release();
        }

        await using var dbConn = new NpgsqlConnection(connectionString);
        await dbConn.OpenAsync();
        await _respawner!.ResetAsync(dbConn);
    }

    protected HttpClient CreateClientAsRole(string role)
    {
        var claims = CreateClaimsForRole(role);
        var client = HttpClient;
        SetClaimsForClient(client, claims);
        return client;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return HttpClient;
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

    protected async Task<T?> ReadFromJsonAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>();
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
}