using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class PromptTemplatesApiTests : AITestBase
{
    public PromptTemplatesApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    private record CreatePromptBody(
        string Name,
        string? Description,
        PromptType Type,
        string TemplateContent,
        string? Variables,
        string? Tags,
        string? Category);

    private record UpdatePromptBody(
        string? Name,
        string? Description,
        string? TemplateContent,
        string? Variables,
        string? Tags,
        string? Category);

    [Fact]
    public async Task CreatePrompt_RequiresAuthentication()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody("Test Prompt", null, PromptType.System, "You are a helpful system.", null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreatePrompt_AsNonAdministrator_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody("Test Prompt", null, PromptType.System, "You are a helpful system.", null, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreatePrompt_AsAIAdministrator_ReturnsCreated_AndCanBeFetchedById()
    {
        var adminClient = CreateClientAsAIAAdministrator();
        var name = $"Coaching Prompt {Guid.NewGuid():N}";

        var createResponse = await adminClient.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody(name, "A coaching prompt", PromptType.Template,
                "Act as a {sport} coach.", null, null, "coaching"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var promptId = await ReadCreatedIdAsync(createResponse);

        var getResponse = await CreateAnonymousClient().GetAsync($"api/v1/prompts/{promptId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var prompt = await ReadOkAsync<PromptTemplateDto>(getResponse);
        prompt.Id.Should().Be(promptId);
        prompt.Name.Should().Be(name);
        prompt.Status.Should().Be(PromptStatus.Draft);
        prompt.CurrentVersion.Should().Be(1);
        prompt.Versions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPrompt_ByMissingId_ReturnsNotFound()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync($"api/v1/prompts/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task SearchPrompts_BySearchTerm_FindsCreatedPrompt()
    {
        var adminClient = CreateClientAsAIAAdministrator();
        var unique = Guid.NewGuid().ToString("N");
        var name = $"Searchable Prompt {unique}";

        await adminClient.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody(name, null, PromptType.System, "Be concise.", null, null, null));

        var searchResponse = await CreateAnonymousClient().GetAsync($"api/v1/prompts?searchTerm={unique}");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(searchResponse);
        total.Should().BeGreaterThanOrEqualTo(1);
        var items = await ReadItemsAsync<PromptSummaryDto>(searchResponse);
        items.Should().Contain(p => p.Name == name);
    }

    [Fact]
    public async Task PublishPrompt_OnDraft_TransitionsToActive()
    {
        var adminClient = CreateClientAsAIAAdministrator();
        var name = $"Publish Prompt {Guid.NewGuid():N}";

        var createResponse = await adminClient.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody(name, null, PromptType.Template, "Publish me.", null, null, null));
        var promptId = await ReadCreatedIdAsync(createResponse);

        var publishResponse = await adminClient.PostAsync($"api/v1/prompts/{promptId}/publish", null);
        publishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var published = await ReadOkAsync<PromptTemplateDto>(publishResponse);
        published.Id.Should().Be(promptId);
        published.Status.Should().Be(PromptStatus.Active);
    }

    [Fact]
    public async Task UpdatePrompt_OnExistingPrompt_IncrementsCurrentVersion()
    {
        var adminClient = CreateClientAsAIAAdministrator();
        var name = $"Versioned Prompt {Guid.NewGuid():N}";

        var createResponse = await adminClient.PostAsJsonAsync("api/v1/prompts",
            new CreatePromptBody(name, null, PromptType.Template, "Version one.", null, null, null));
        var promptId = await ReadCreatedIdAsync(createResponse);

        var updateResponse = await adminClient.PutAsJsonAsync($"api/v1/prompts/{promptId}",
            new UpdatePromptBody(null, null, "Version two.", null, null, null));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadOkAsync<PromptTemplateDto>(updateResponse);
        updated.Id.Should().Be(promptId);
        updated.CurrentVersion.Should().Be(2);
    }
}
