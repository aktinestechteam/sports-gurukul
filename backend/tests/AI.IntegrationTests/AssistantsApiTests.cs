using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class AssistantsApiTests : AITestBase
{
    public AssistantsApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    private record CreateAssistantBody(
        string Name,
        string? Description,
        AIAssistantType AssistantType,
        AIAssistantPersonality Personality,
        string? SystemPrompt,
        string? GreetingMessage,
        bool IsPublic);

    private record UpdateAssistantBody(
        string? Name,
        string? Description,
        AIAssistantType? AssistantType,
        AIAssistantPersonality? Personality,
        string? SystemPrompt,
        string? GreetingMessage,
        bool? IsPublic);

    [Fact]
    public async Task CreateAssistant_RequiresAuthentication()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/assistants",
            new CreateAssistantBody("Test Assistant", null, AIAssistantType.General,
                AIAssistantPersonality.Professional, null, null, false));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAssistant_ReturnsCreated_AndCanBeFetchedById()
    {
        var client = CreateClientAsStandardUser();
        var name = $"Training Assistant {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/assistants",
            new CreateAssistantBody(name, "Helps with training", AIAssistantType.Coach,
                AIAssistantPersonality.Motivational, "You are a helpful coach.", "Welcome!", false));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var assistantId = await ReadCreatedIdAsync(createResponse);

        var getResponse = await client.GetAsync($"api/v1/assistants/{assistantId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var assistant = await ReadOkAsync<AssistantDto>(getResponse);
        assistant.Id.Should().Be(assistantId);
        assistant.Name.Should().Be(name);
        assistant.AssistantType.Should().Be(AIAssistantType.Coach);
        assistant.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetAssistant_ByMissingId_ReturnsNotFound()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.GetAsync($"api/v1/assistants/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task UpdateAssistant_OnExistingAssistant_ChangesName()
    {
        var client = CreateClientAsStandardUser();
        var name = $"Rename Me {Guid.NewGuid():N}";
        var newName = $"Renamed {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/assistants",
            new CreateAssistantBody(name, null, AIAssistantType.Mentor,
                AIAssistantPersonality.Friendly, null, null, false));
        var assistantId = await ReadCreatedIdAsync(createResponse);

        var updateResponse = await client.PutAsJsonAsync($"api/v1/assistants/{assistantId}",
            new UpdateAssistantBody(newName, "Updated description", null, null, null, null, null));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadOkAsync<AssistantDto>(updateResponse);
        updated.Id.Should().Be(assistantId);
        updated.Name.Should().Be(newName);
    }

    [Fact]
    public async Task UpdateAssistant_OnMissingAssistant_ReturnsNotFound()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.PutAsJsonAsync($"api/v1/assistants/{Guid.NewGuid()}",
            new UpdateAssistantBody("New Name", null, null, null, null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task SearchAssistants_BySearchTerm_FindsCreatedAssistant()
    {
        var client = CreateClientAsStandardUser();
        var unique = Guid.NewGuid().ToString("N");
        var name = $"Searchable Assistant {unique}";

        await client.PostAsJsonAsync("api/v1/assistants",
            new CreateAssistantBody(name, null, AIAssistantType.General,
                AIAssistantPersonality.Professional, null, null, false));

        var searchResponse = await client.GetAsync($"api/v1/assistants?searchTerm={unique}");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(searchResponse);
        total.Should().BeGreaterThanOrEqualTo(1);
        var items = await ReadItemsAsync<AssistantSummaryDto>(searchResponse);
        items.Should().Contain(a => a.Name == name);
    }

    [Fact]
    public async Task PublishAssistant_OnExistingAssistant_ReturnsOk()
    {
        var client = CreateClientAsStandardUser();
        var name = $"Publish Me {Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/assistants",
            new CreateAssistantBody(name, null, AIAssistantType.Scout,
                AIAssistantPersonality.Analytical, null, null, false));
        var assistantId = await ReadCreatedIdAsync(createResponse);

        var publishResponse = await client.PostAsync($"api/v1/assistants/{assistantId}/publish", null);
        publishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var published = await ReadOkAsync<AssistantDto>(publishResponse);
        published.Id.Should().Be(assistantId);
        published.IsActive.Should().BeTrue();
    }
}
