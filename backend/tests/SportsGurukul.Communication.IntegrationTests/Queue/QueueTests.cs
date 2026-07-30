using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace SportsGurukul.Communication.IntegrationTests.Queue;

public class QueueTests : CommunicationTestBase
{
    public QueueTests(CommunicationTestApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task GetQueue_AsAdmin_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/queue");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetQueue_AsAthlete_Returns403()
    {
        var client = CreateAuthenticatedClient("Athlete");

        var response = await GetAsync(client, "/api/v1/queue");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetQueue_WithoutAuth_Returns401()
    {
        var client = CreateAnonymousClient();

        var response = await GetAsync(client, "/api/v1/queue");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFailedQueue_AsAdmin_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");

        var response = await GetAsync(client, "/api/v1/queue/failed");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReprocessQueue_WithValidIds_Returns200()
    {
        var client = CreateAuthenticatedClient("Admin");
        var request = new { NotificationIds = new[] { Guid.NewGuid(), Guid.NewGuid() } };

        var response = await PostJsonAsync(client, "/api/v1/queue/reprocess", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReprocessQueue_WithEmptyIds_Returns400()
    {
        var client = CreateAuthenticatedClient("Admin");
        var request = new { NotificationIds = Array.Empty<Guid>() };

        var response = await PostJsonAsync(client, "/api/v1/queue/reprocess", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
