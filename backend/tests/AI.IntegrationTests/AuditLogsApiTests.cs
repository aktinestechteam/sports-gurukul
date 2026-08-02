using System.Net;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class AuditLogsApiTests : AITestBase
{
    public AuditLogsApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAuditLogs_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync("api/v1/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAuditLogs_StandardUser_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.GetAsync("api/v1/audit-logs");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAuditLogs_ReturnsPagedEnvelope()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/audit-logs?page=1&pageSize=20");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<AuditLogDto>(response);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAuditLogs_FilterByEventType_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/audit-logs?eventType={AuditEventType.Create}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<AuditLogDto>(response);
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAuditLogs_FilterByEntityTypeAndSeverity_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/audit-logs?entityType=Agent&severity={AuditSeverity.Info}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<AuditLogDto>(response);
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAuditLogs_FilterByDateRange_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/audit-logs?fromDate=2025-01-01&toDate=2035-12-31");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
    }
}
