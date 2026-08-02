using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class AgentsApiTests : AITestBase
{
    public AgentsApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task CreateAgent_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest("Scouting Agent", null, null, null, null, null, null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAgent_StandardUser_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest("Scouting Agent", null, null, null, null, null, null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateAgent_ReturnsCreated_WithDraftStatusAndConfiguration()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Agent {Guid.NewGuid():N}";

        var response = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest(name, "Autonomous scouting agent", null,
                "{\"strategy\":\"conservative\"}", "knowledge_search", "Always confirm decisions",
                "Max 10 tool calls per run", 5, true));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = (await ReadApiResponseAsync<AgentDto>(response, HttpStatusCode.Created)).Data;
        created.Should().NotBeNull();
        created!.Id.Should().NotBe(Guid.Empty);
        created.Name.Should().Be(name);
        created.Description.Should().Be("Autonomous scouting agent");
        created.Status.Should().Be(AgentStatus.Draft);
        created.MaxIterations.Should().Be(5);
        created.RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public async Task GetAgent_ById_ReturnsMatchingAgent()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Agent {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest(name, "Round trip agent", null, null, null, null, null, null, null));
        var id = await ReadCreatedIdAsync(createResponse);

        var getResponse = await client.GetAsync($"api/v1/agents/{id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var agent = await ReadOkAsync<AgentDto>(getResponse);
        agent.Id.Should().Be(id);
        agent.Name.Should().Be(name);
        agent.Status.Should().Be(AgentStatus.Draft);
    }

    [Fact]
    public async Task GetAgent_MissingId_ReturnsNotFound()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync($"api/v1/agents/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateAgent_UpdatesName_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Agent {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest(name, "Before update", null, null, null, null, null, null, null));
        var id = await ReadCreatedIdAsync(createResponse);

        var updatedName = $"{name} - v2";
        var updateResponse = await client.PutAsJsonAsync($"api/v1/agents/{id}",
            new UpdateAgentRequest(updatedName, "After update", null, null, null, null, null, null));

        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadOkAsync<AgentDto>(updateResponse);
        updated.Id.Should().Be(id);
        updated.Name.Should().Be(updatedName);
        updated.Description.Should().Be("After update");
    }

    [Fact]
    public async Task EnableAndDisableAgent_ChangesStatus()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"Scouting Agent {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest(name, null, null, null, null, null, null, null, null));
        var id = await ReadCreatedIdAsync(createResponse);

        var enableResponse = await client.PostAsync($"api/v1/agents/{id}/enable", null);
        enableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var enabled = await ReadOkAsync<AgentDto>(enableResponse);
        enabled.Id.Should().Be(id);
        enabled.Status.Should().Be(AgentStatus.Active);

        var disableResponse = await client.PostAsync($"api/v1/agents/{id}/disable", null);
        disableResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var disabled = await ReadOkAsync<AgentDto>(disableResponse);
        disabled.Id.Should().Be(id);
        disabled.Status.Should().Be(AgentStatus.Inactive);
    }

    [Fact]
    public async Task AssignWorkflow_ToExistingAgent_ReturnsOk()
    {
        var client = CreateClientAsAIAAdministrator();
        var agentName = $"Scouting Agent {Guid.NewGuid():N}";
        var workflowName = $"Scouting Workflow {Guid.NewGuid():N}";

        var createAgentResponse = await client.PostAsJsonAsync("api/v1/agents",
            new CreateAgentRequest(agentName, null, null, null, null, null, null, null, null));
        var agentId = await ReadCreatedIdAsync(createAgentResponse);

        var createWorkflowResponse = await client.PostAsJsonAsync("api/v1/workflows",
            new CreateWorkflowRequest(workflowName, null, null, null, null, null));
        var workflowId = await ReadCreatedIdAsync(createWorkflowResponse);

        var response = await client.PostAsJsonAsync($"api/v1/agents/{agentId}/workflow",
            new AssignWorkflowRequest(workflowId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchAgents_ReturnsPagedEnvelope()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.GetAsync("api/v1/agents?status=Active&page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<AgentSummaryDto>(response);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(0);
        items.Should().NotBeNull();
    }
}
