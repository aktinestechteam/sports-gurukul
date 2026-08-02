using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class MessagesApiTests : AITestBase
{
    public MessagesApiTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task AddMessage_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{Guid.NewGuid()}/messages",
            new AddMessageRequest(MessageRole.User, "Hello", null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMessageHistory_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.GetAsync($"api/v1/conversations/{Guid.NewGuid()}/messages");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AddMessage_ToExistingConversation_ReturnsOkWithIncrementedMessageCount()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);
        var conversationId = await CreateConversationAsync(client, $"Messages {Guid.NewGuid():N}");

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{conversationId}/messages",
            new AddMessageRequest(MessageRole.User, "What drills should I run today?", null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var conversation = await ReadOkAsync<ConversationDto>(response);
        conversation.Id.Should().Be(conversationId);
        conversation.MessageCount.Should().Be(1);
    }

    [Fact]
    public async Task GetMessageHistory_ReturnsPagedMessageItems()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);
        var conversationId = await CreateConversationAsync(client, $"History {Guid.NewGuid():N}");

        await client.PostAsJsonAsync($"api/v1/conversations/{conversationId}/messages",
            new AddMessageRequest(MessageRole.User, "Plan my recovery week", null));
        await client.PostAsJsonAsync($"api/v1/conversations/{conversationId}/messages",
            new AddMessageRequest(MessageRole.Assistant, "Here is your recovery plan.", null));

        var response = await client.GetAsync($"api/v1/conversations/{conversationId}/messages?page=1&pageSize=50");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await ReadItemsAsync<MessageDto>(response);
        var total = await ReadTotalCountAsync(response);
        total.Should().Be(2);
        history.Should().Contain(m => m.Role == MessageRole.User && m.Content.Contains("recovery"));
        history.Should().Contain(m => m.Role == MessageRole.Assistant);
        history.Should().OnlyContain(m => m.ConversationId == conversationId);
    }

    [Fact]
    public async Task GetMessageHistory_ForMissingConversation_ReturnsNotFound()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.GetAsync($"api/v1/conversations/{Guid.NewGuid()}/messages");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task AddMessage_ToMissingConversation_ReturnsNotFound()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{Guid.NewGuid()}/messages",
            new AddMessageRequest(MessageRole.User, "Hello", null));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task RegenerateResponse_Anonymous_ReturnsUnauthorized()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{Guid.NewGuid()}/regenerate",
            new RegenerateRequest(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RegenerateResponse_ReturnsBadRequest_NotYetImplemented()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);
        var conversationId = await CreateConversationAsync(client, $"Regenerate {Guid.NewGuid():N}");

        await client.PostAsJsonAsync($"api/v1/conversations/{conversationId}/messages",
            new AddMessageRequest(MessageRole.User, "Ask me a follow-up question", null));

        var historyResponse = await client.GetAsync($"api/v1/conversations/{conversationId}/messages?page=1&pageSize=10");
        var history = await ReadItemsAsync<MessageDto>(historyResponse);
        var messageId = history.First(m => m.Role == MessageRole.User).Id;

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{conversationId}/regenerate",
            new RegenerateRequest(messageId));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not yet implemented");
    }

    private async Task<Guid> CreateConversationAsync(HttpClient client, string title)
    {
        var createResponse = await client.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationRequest(title, null));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        return await ReadCreatedIdAsync(createResponse);
    }
}
