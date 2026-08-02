using System.Net;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using Xunit;

namespace AI.IntegrationTests;

public class TokenUsageApiTests : AITestBase
{
    public TokenUsageApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetTokenUsage_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync("api/v1/token-usage");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTokenUsage_StandardUser_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.GetAsync("api/v1/token-usage");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetTokenUsage_ReturnsPagedEnvelope()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/token-usage?page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<TokenUsageSummaryDto>(response);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTokenUsage_FilterByDateRange_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/token-usage?fromDate=2025-01-01&toDate=2035-12-31");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<TokenUsageSummaryDto>(response);
        items.Should().NotBeNull();
        foreach (var item in items)
            item.CreatedAt.Should().BeOnOrAfter(new DateTime(2025, 1, 1));
    }

    [Fact]
    public async Task GetTokenUsage_FilterByConversation_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/token-usage?conversationId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetTokenUsage_FilterByUserId_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/token-usage?userId={AITestIds.AthleteUserId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
    }
}
