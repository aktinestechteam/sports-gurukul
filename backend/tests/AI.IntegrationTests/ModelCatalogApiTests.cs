using System.Net;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class ModelCatalogApiTests : AITestBase
{
    public ModelCatalogApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetModels_RequiresAuthentication()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync("api/v1/models");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetModels_ReturnsSeededCatalog_IncludingGpt4o()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.GetAsync("api/v1/models?pageSize=50");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var total = await ReadTotalCountAsync(response);
        total.Should().BeGreaterThanOrEqualTo(4);
        var items = await ReadItemsAsync<ModelCatalogDto>(response);
        items.Should().Contain(m => m.Name == "gpt-4o");
    }

    [Fact]
    public async Task GetModels_SearchTermGpt_ReturnsOnlyMatchingModels()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.GetAsync("api/v1/models?searchTerm=gpt&pageSize=50");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<ModelCatalogDto>(response);
        items.Should().NotBeEmpty();
        items.Should().OnlyContain(m =>
            (m.Name ?? string.Empty).Contains("gpt", StringComparison.OrdinalIgnoreCase) ||
            (m.DisplayName ?? string.Empty).Contains("gpt", StringComparison.OrdinalIgnoreCase));
        items.Should().Contain(m => m.Name == "gpt-4o");
    }

    [Fact]
    public async Task GetModels_ByProviderId_ReturnsOnlyModelsFromThatProvider()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.GetAsync($"api/v1/models?providerId={AITestIds.OpenAiProviderId}&pageSize=50");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<ModelCatalogDto>(response);
        items.Should().NotBeEmpty();
        items.Should().OnlyContain(m => m.ProviderId == AITestIds.OpenAiProviderId);
        items.Should().Contain(m => m.Name == "gpt-4o");
    }

    [Fact]
    public async Task GetModels_ActiveOnly_ExcludesDeprecatedAndPreviewModels()
    {
        var client = CreateClientAsStandardUser();

        var response = await client.GetAsync("api/v1/models?activeOnly=true&pageSize=50");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await ReadItemsAsync<ModelCatalogDto>(response);
        items.Should().NotBeEmpty();
        items.Should().OnlyContain(m => m.Status == AIModelStatus.Active);
        items.Should().Contain(m => m.Name == "gpt-4o");
        items.Should().NotContain(m => m.Name == "gpt-3.5-turbo");
    }
}
