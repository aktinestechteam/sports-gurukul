using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace SportsGurukul.Communication.IntegrationTests;

public abstract class CommunicationTestBase : IClassFixture<CommunicationTestApplicationFactory>
{

    protected CommunicationTestApplicationFactory Factory { get; }

    protected CommunicationTestBase(CommunicationTestApplicationFactory factory)
    {
        Factory = factory;
    }

    protected HttpClient HttpClient =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

    protected MockBusinessRuleValidator RuleValidator => Factory.RuleValidator;
    protected MockNotificationDispatcher Dispatcher => Factory.Dispatcher;
    protected MockQueueService QueueService => Factory.QueueService;

    protected HttpClient CreateAuthenticatedClient(string role = "Admin")
    {
        var client = Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var (userId, email, name) = role switch
        {
            "Admin" => ("A0000000-0000-0000-0000-000000000001", "admin@sportsgurukul.com", "Admin User"),
            "SuperAdmin" => ("A0000000-0000-0000-0000-000000000002", "superadmin@sportsgurukul.com", "Super Admin"),
            "Communication Admin" => ("A0000000-0000-0000-0000-000000000003", "commadmin@sportsgurukul.com", "Comm Admin"),
            "Academy Admin" => ("A0000000-0000-0000-0000-000000000004", "academyadmin@sportsgurukul.com", "Academy Admin"),
            "Athlete" => ("30000000-0000-0000-0000-000000000001", "athlete@sportsgurukul.com", "Test Athlete"),
            _ => ("A0000000-0000-0000-0000-000000000001", "admin@sportsgurukul.com", "Admin User")
        };

        var token = GenerateJwtToken(Guid.Parse(userId), email, name, new[] { role });
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return Factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    private static string GenerateJwtToken(Guid userId, string email, string fullName, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("full_name", fullName),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CommunicationTestApplicationFactory.TestJwtSigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "SportsGurukul",
            audience: "SportsGurukul",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected async Task<HttpResponseMessage> PostJsonAsync<T>(HttpClient client, string url, T body)
        => await client.PostAsJsonAsync(url, body);

    protected async Task<HttpResponseMessage> PutJsonAsync<T>(HttpClient client, string url, T body)
        => await client.PutAsJsonAsync(url, body);

    protected async Task<HttpResponseMessage> GetAsync(HttpClient client, string url)
        => await client.GetAsync(url);

    protected async Task<HttpResponseMessage> DeleteAsync(HttpClient client, string url)
        => await client.DeleteAsync(url);
}
