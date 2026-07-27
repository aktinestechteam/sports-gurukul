using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Booking.IntegrationTests;

public static class AuthenticatedHttpClientFactory
{
    private const string JwtSecretKey = "TestSecretKeyForIntegrationTests12345678!";
    private const string JwtIssuer = "SportsGurukul";
    private const string JwtAudience = "SportsGurukul";

    public static HttpClient CreateClientWithJwt(HttpClient client, string jwtToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        return client;
    }

    public static HttpClient CreateClientWithRole(HttpClient client, string role)
    {
        var token = GenerateJwtToken(Guid.NewGuid(), "test@test.com", "Test User", role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public static HttpClient CreateAnonymousClient(HttpClient client)
    {
        return client;
    }

    public static HttpClient CreateClientWithClaims(
        HttpClient client,
        Guid userId,
        string email,
        string displayName,
        string role)
    {
        var token = GenerateJwtToken(userId, email, displayName, role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string GenerateJwtToken(
        Guid userId,
        string email,
        string displayName,
        string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, displayName),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
