using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class WorkflowsApiTests : AITestBase
{
    public WorkflowsApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateWorkflow_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest("Scouting Workflow", null, null, null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateWorkflow_StandardUser_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest("Scouting Workflow", null, null, null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateWorkflow_ReturnsCreated_WithDraftStatusAndVersionOne()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Workflow {Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest(name, "Detect player talent",
                "1. collect metrics; 2. evaluate", "on-player-match", "match.rating > 80", "minAge=14"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = (await ReadApiResponseAsync<WorkflowDto>(response, HttpStatusCode.Created)).Data;
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.Name.Should().Be(name);
        created.Status.Should().Be(WorkflowStatus.Draft);
        created.Version.Should().Be(1);
        created.Steps.Should().Contain("collect metrics");
    }

    [Fact]
    public async Task GetWorkflow_ById_ReturnsMatchingWorkflow()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Workflow {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest(name, "Round trip workflow", null, null, null, null));
        var id = await ReadCreatedIdAsync(createResponse);

        var getResponse = await client.GetAsync($"api/v1/workflows/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var workflow = await ReadOkAsync<WorkflowDto>(getResponse);
        workflow.Id.Should().Be(id);
        workflow.Name.Should().Be(name);
        workflow.Status.Should().Be(WorkflowStatus.Draft);
    }

    [Fact]
    public async Task GetWorkflow_MissingId_ReturnsNotFound()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/workflows/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateWorkflow_IncrementsVersion_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Workflow {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest(name, "Before update", null, null, null, null));
        var id = await ReadCreatedIdAsync(createResponse);

        var updatedName = $"{name} - v2";
        var updateResponse = await client.PutAsJsonAsync($"api/v1/workflows/{id}",
            new UpdateWorkflowRequest(updatedName, "After update", "1. scout; 2. report", null, null, null));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadOkAsync<WorkflowDto>(updateResponse);
        updated.Id.Should().Be(id);
        updated.Name.Should().Be(updatedName);
        updated.Description.Should().Be("After update");
        updated.Steps.Should().Be("1. scout; 2. report");
        updated.Version.Should().Be(2);
    }

    [Fact]
    public async Task SearchWorkflows_ReturnsPagedEnvelope()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/workflows?status=Draft&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<WorkflowSummaryDto>(response);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
        items.Should().NotBeNull();
    }
}
