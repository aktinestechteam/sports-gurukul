using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class KnowledgeBasesApiTests : AITestBase
{
    public KnowledgeBasesApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    private record CreateKnowledgeBaseBody(
        string Name, string? Description, KnowledgeBaseVisibility Visibility,
        string? Category, string? Tags);

    private record UpdateKnowledgeBaseBody(
        string? Name, string? Description, KnowledgeBaseVisibility? Visibility,
        string? Category, string? Tags);

    [Fact]
    public async Task CreateKnowledgeBase_AsAnonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody("Anonymous KB", null, KnowledgeBaseVisibility.Public, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateKnowledgeBase_AsStandardUser_ReturnsForbidden()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody("Standard User KB", null, KnowledgeBaseVisibility.Public, null, null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateKnowledgeBase_WithInvalidBody_ReturnsBadRequest()
    {
        var client = CreateClientAsAIAAdministrator();

        var response = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new { Name = (string?)null, Visibility = KnowledgeBaseVisibility.Public });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateKnowledgeBase_ThenGetById_ReturnsCreatedKnowledgeBase()
    {
        var client = CreateClientAsAIAAdministrator();
        var name = $"KB-{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody(name, "Integration test knowledge base",
                KnowledgeBaseVisibility.Public, "Sports", "cricket,rules"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var knowledgeBaseId = await ReadCreatedIdAsync(createResponse);

        var getResponse = await client.GetAsync($"api/v1/knowledge-bases/{knowledgeBaseId}");
        var knowledgeBase = await ReadOkAsync<KnowledgeBaseDto>(getResponse);
        knowledgeBase.Id.Should().Be(knowledgeBaseId);
        knowledgeBase.Name.Should().Be(name);
        knowledgeBase.Description.Should().Be("Integration test knowledge base");
        knowledgeBase.Visibility.Should().Be(KnowledgeBaseVisibility.Public);
        knowledgeBase.Category.Should().Be("Sports");
        knowledgeBase.Status.Should().Be(KnowledgeBaseStatus.Draft);
    }

    [Fact]
    public async Task GetKnowledgeBase_WithMissingId_ReturnsNotFound()
    {
        var client = CreateClientAsAIAAdministrator();
        var missingId = Guid.NewGuid();

        var response = await client.GetAsync($"api/v1/knowledge-bases/{missingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task SearchKnowledgeBases_FiltersByUniqueSearchTerm()
    {
        var client = CreateClientAsAIAAdministrator();
        var uniqueName = $"KB-Search-{Guid.NewGuid():N}";

        var createResponse = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody(uniqueName, "Searchable knowledge base",
                KnowledgeBaseVisibility.Public, null, null));
        var knowledgeBaseId = await ReadCreatedIdAsync(createResponse);

        var searchResponse = await client.GetAsync(
            $"api/v1/knowledge-bases?searchTerm={Uri.EscapeDataString(uniqueName)}");
        searchResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var totalCount = await ReadTotalCountAsync(searchResponse);
        totalCount.Should().BeGreaterThanOrEqualTo(1);
        var items = await ReadItemsAsync<KnowledgeBaseSummaryDto>(searchResponse);
        items.Should().Contain(i => i.Id == knowledgeBaseId && i.Name == uniqueName);
    }

    [Fact]
    public async Task UpdateKnowledgeBase_ThenGetById_ReflectsNewName()
    {
        var client = CreateClientAsAIAAdministrator();
        var originalName = $"KB-Update-{Guid.NewGuid():N}";
        var newName = $"{originalName}-Renamed";

        var createResponse = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody(originalName, null, KnowledgeBaseVisibility.Team, null, null));
        var knowledgeBaseId = await ReadCreatedIdAsync(createResponse);

        var updateResponse = await client.PutAsJsonAsync($"api/v1/knowledge-bases/{knowledgeBaseId}",
            new UpdateKnowledgeBaseBody(newName, "Updated description",
                KnowledgeBaseVisibility.Public, "Sports", "rules"));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await ReadOkAsync<KnowledgeBaseDto>(updateResponse);
        updated.Id.Should().Be(knowledgeBaseId);
        updated.Name.Should().Be(newName);
        updated.Visibility.Should().Be(KnowledgeBaseVisibility.Public);
    }
}
