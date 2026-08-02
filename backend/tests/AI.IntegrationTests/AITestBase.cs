using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using AI.IntegrationTests.Fixtures;
using AI.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using SportsGurukul.Api.Common.Models;
using FluentAssertions;
using Xunit;

namespace AI.IntegrationTests;

public abstract class AITestBase : IClassFixture<AICustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    protected AICustomWebApplicationFactory Factory { get; }

    protected AITestBase(AICustomWebApplicationFactory factory)
    {
        Factory = factory;
    }

    protected HttpClient CreateAnonymousClient()
    {
        return CreateClientWithTestAuth(null);
    }

    protected HttpClient CreateClientAsPlatformAdministrator(Guid? userId = null)
    {
        var claims = TestAuthExtensions.CreateClaimsForUser(
            userId ?? AITestIds.AdminUserId,
            "platform-admin@test.com",
            "Platform Admin",
            AIRoleNames.PlatformAdministrator);
        return CreateClientWithClaims(claims);
    }

    protected HttpClient CreateClientAsAIAAdministrator(Guid? userId = null)
    {
        var claims = TestAuthExtensions.CreateClaimsForUser(
            userId ?? AITestIds.AdminUserId,
            "ai-admin@test.com",
            "AI Admin",
            AIRoleNames.AIAdministrator);
        return CreateClientWithClaims(claims);
    }

    protected HttpClient CreateClientAsStandardUser(Guid? userId = null)
    {
        var claims = TestAuthExtensions.CreateClaimsForUser(
            userId ?? AITestIds.AthleteUserId,
            "standard-user@test.com",
            "Standard User");
        return CreateClientWithClaims(claims);
    }

    protected HttpClient CreateClientAsCoach(Guid? userId = null)
    {
        var claims = TestAuthExtensions.CreateClaimsForUser(
            userId ?? AITestIds.CoachUserId,
            "coach@test.com",
            "Coach User");
        return CreateClientWithClaims(claims);
    }

    private HttpClient CreateClientWithClaims(IEnumerable<Claim> claims)
    {
        var client = CreateClientWithTestAuth(null);
        TestAuthExtensions.SetTestClaimsHeader(client, claims);
        return client;
    }

    private HttpClient CreateClientWithTestAuth(IEnumerable<Claim>? initialClaims)
    {
        var client = Factory.CreateClient();

        if (initialClaims is not null)
            TestAuthExtensions.SetTestClaimsHeader(client, initialClaims);

        return client;
    }

    protected static async Task<ApiResponse<T>> ReadApiResponseAsync<T>(
        HttpResponseMessage response, HttpStatusCode expectedStatus = HttpStatusCode.OK)
    {
        response.StatusCode.Should().Be(expectedStatus);
        var body = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(body, JsonOptions);
        apiResponse.Should().NotBeNull();
        return apiResponse!;
    }

    protected static async Task<Guid> ReadCreatedIdAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var id = doc.RootElement.GetProperty("data").GetProperty("id").GetGuid();
        return id;
    }

    protected static async Task<Guid> ReadOkIdAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var id = doc.RootElement.GetProperty("data").GetProperty("id").GetGuid();
        return id;
    }

    protected static async Task<int> ReadTotalCountAsync(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32();
    }

    protected static async Task<string> ReadDetailAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        if (doc.RootElement.TryGetProperty("detail", out var detail))
            return detail.GetString() ?? string.Empty;
        if (doc.RootElement.TryGetProperty("errors", out var errors) &&
            errors.ValueKind == JsonValueKind.Array && errors.GetArrayLength() > 0)
            return errors[0].GetString() ?? string.Empty;
        return body;
    }

    protected static async Task<JsonElement> ReadDataAsync(HttpResponseMessage response, HttpStatusCode expectedStatus = HttpStatusCode.OK)
    {
        response.StatusCode.Should().Be(expectedStatus);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("data").Clone();
    }

    protected static async Task<List<T>> ReadItemsAsync<T>(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var itemsElement = doc.RootElement.GetProperty("data").GetProperty("items");
        return JsonSerializer.Deserialize<List<T>>(itemsElement.GetRawText(), JsonOptions) ?? [];
    }

    protected static async Task<T> ReadOkAsync<T>(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var dataElement = doc.RootElement.GetProperty("data");
        return JsonSerializer.Deserialize<T>(dataElement.GetRawText(), JsonOptions)!;
    }

    protected static async Task<HttpResponseMessage> PostJsonAsync<T>(HttpClient client, string url, T body)
        => await client.PostAsJsonAsync(url, body);

    protected static async Task<HttpResponseMessage> PutJsonAsync<T>(HttpClient client, string url, T body)
        => await client.PutAsJsonAsync(url, body);
}
