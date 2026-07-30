using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Communication.IntegrationTests.Delivery;

public class DeliveryTests : CommunicationTestBase
{
    public DeliveryTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetDelivery_WithNotificationId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client,
            $"/api/v1/delivery?notificationId={Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDelivery_WithoutNotificationId_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/delivery");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDeliveryById_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, $"/api/v1/delivery/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetDelivery_WithoutAuth_Returns401()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "/api/v1/delivery?notificationId=" + Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetStatistics_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/delivery/statistics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<NotificationStatisticsDto>>();
        content.Should().NotBeNull();
        content!.Success.Should().BeTrue();
    }

    [Fact]
    public async Task GetStatistics_WithFilters_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var fromDate = DateTime.UtcNow.AddDays(-7);
        var toDate = DateTime.UtcNow;

        var response = await GetAsync(client,
            $"/api/v1/delivery/statistics?fromDate={fromDate:O}&toDate={toDate:O}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetStatistics_AsAnonymous_Returns200()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "/api/v1/delivery/statistics");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
