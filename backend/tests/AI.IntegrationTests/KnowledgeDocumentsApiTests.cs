using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class KnowledgeDocumentsApiTests : AITestBase
{
    public KnowledgeDocumentsApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    private record CreateKnowledgeBaseBody(
        string Name, string? Description, KnowledgeBaseVisibility Visibility,
        string? Category, string? Tags);

    private async Task<Guid> CreateKnowledgeBaseAsync(string name)
    {
        var client = CreateClientAsAIAAdministrator();
        var response = await client.PostAsJsonAsync("api/v1/knowledge-bases",
            new CreateKnowledgeBaseBody(name, "Parent knowledge base for documents",
                KnowledgeBaseVisibility.Public, null, null));
        return await ReadCreatedIdAsync(response);
    }

    [Fact]
    public async Task GetDocuments_AsAnonymous_IsAllowedAndReturnsEmptyList()
    {
        var knowledgeBaseId = await CreateKnowledgeBaseAsync($"KB-Docs-{Guid.NewGuid():N}");
        var anonymousClient = CreateAnonymousClient();

        var response = await anonymousClient.GetAsync(
            $"api/v1/knowledge-bases/{knowledgeBaseId}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await ReadApiResponseAsync<List<KnowledgeDocumentDto>>(response);
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDocuments_ForMissingKnowledgeBase_ReturnsNotFound()
    {
        var client = CreateClientAsAIAAdministrator();
        var missingId = Guid.NewGuid();

        var response = await client.GetAsync($"api/v1/knowledge-bases/{missingId}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task GetDocuments_ForExistingKnowledgeBase_ReturnsEmptyList()
    {
        var client = CreateClientAsAIAAdministrator();
        var knowledgeBaseId = await CreateKnowledgeBaseAsync($"KB-Docs-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"api/v1/knowledge-bases/{knowledgeBaseId}/documents");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await ReadApiResponseAsync<List<KnowledgeDocumentDto>>(response);
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDocuments_AcceptsPaginationQueryParameters()
    {
        var client = CreateClientAsAIAAdministrator();
        var knowledgeBaseId = await CreateKnowledgeBaseAsync($"KB-Docs-{Guid.NewGuid():N}");

        var response = await client.GetAsync(
            $"api/v1/knowledge-bases/{knowledgeBaseId}/documents?page=1&pageSize=10");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var apiResponse = await ReadApiResponseAsync<List<KnowledgeDocumentDto>>(response);
        apiResponse.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDocuments_ResponseData_IsJsonArrayNotPaginatedEnvelope()
    {
        var client = CreateClientAsAIAAdministrator();
        var knowledgeBaseId = await CreateKnowledgeBaseAsync($"KB-Docs-{Guid.NewGuid():N}");

        var response = await client.GetAsync($"api/v1/knowledge-bases/{knowledgeBaseId}/documents");
        var body = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(body);
        var dataElement = doc.RootElement.GetProperty("data");
        dataElement.ValueKind.Should().Be(JsonValueKind.Array);
    }
}
