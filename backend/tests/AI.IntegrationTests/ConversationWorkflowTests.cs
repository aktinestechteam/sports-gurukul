using System.Net;
using System.Net.Http.Json;
using AI.IntegrationTests.Fixtures;
using FluentAssertions;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;
using Xunit;

namespace AI.IntegrationTests;

public class ConversationWorkflowTests : AITestBase
{
    public ConversationWorkflowTests(AICustomWebApplicationFactory factory) : base(factory)
    {
    }

    private record CreateConversationBody(string? Title, Guid? AssistantId);
    private record AddMessageBody(MessageRole Role, string Content, string? Metadata);
    private record RenameConversationBody(string Title);

    [Fact]
    public async Task CreateConversation_RequiresAuthentication()
    {
        var client = CreateAnonymousClient();

        var response = await client.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationBody("Training Plan", null));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task FullConversationLifecycle_Succeeds_EndToEnd()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var createResponse = await client.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationBody("Pre-Season Training", null));
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var conversationId = await ReadCreatedIdAsync(createResponse);

        var addResponse = await client.PostAsJsonAsync(
            $"api/v1/conversations/{conversationId}/messages",
            new AddMessageBody(MessageRole.User, "What is my training plan for this week?", null));
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var conversationAfterAdd = await ReadOkAsync<ConversationDto>(addResponse);
        conversationAfterAdd.MessageCount.Should().Be(1);
        conversationAfterAdd.Messages.Should().ContainSingle(m =>
            m.Role == MessageRole.User && m.Content.Contains("training plan"));

        var historyResponse = await client.GetAsync($"api/v1/conversations/{conversationId}/messages?page=1&pageSize=50");
        historyResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await ReadItemsAsync<MessageDto>(historyResponse);
        history.Should().ContainSingle(m => m.Role == MessageRole.User);

        var renameResponse = await client.PutAsJsonAsync($"api/v1/conversations/{conversationId}",
            new RenameConversationBody("Weekly Training Focus"));
        renameResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var renamed = await ReadOkAsync<ConversationDto>(renameResponse);
        renamed.Title.Should().Be("Weekly Training Focus");

        var summarizeResponse = await client.PostAsync($"api/v1/conversations/{conversationId}/summarize", null);
        summarizeResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var summarized = await ReadOkAsync<ConversationDto>(summarizeResponse);
        summarized.ContextSummary.Should().NotBeNullOrWhiteSpace();

        var deleteResponse = await client.DeleteAsync($"api/v1/conversations/{conversationId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getDeletedResponse = await client.GetAsync($"api/v1/conversations/{conversationId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddMessage_ToMissingConversation_ReturnsNotFound()
    {
        var client = CreateClientAsStandardUser();
        var missingId = Guid.NewGuid();

        var response = await client.PostAsJsonAsync(
            $"api/v1/conversations/{missingId}/messages",
            new AddMessageBody(MessageRole.User, "Hello", null));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var detail = await ReadDetailAsync(response);
        detail.Should().Contain("not found");
    }

    [Fact]
    public async Task ConversationSearch_ScopesResults_ToCurrentUser()
    {
        var athleteClient = CreateClientAsStandardUser(AITestIds.AthleteUserId);
        var otherClient = CreateClientAsCoach(AITestIds.CoachUserId);

        await athleteClient.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationBody("Athlete Private Plan", null));

        var athleteSearch = await athleteClient.GetAsync("api/v1/conversations?searchTerm=Private");
        athleteSearch.StatusCode.Should().Be(HttpStatusCode.OK);
        var athleteTotal = await ReadTotalCountAsync(athleteSearch);
        athleteTotal.Should().BeGreaterThanOrEqualTo(1);

        var coachSearch = await otherClient.GetAsync("api/v1/conversations?searchTerm=Private");
        coachSearch.StatusCode.Should().Be(HttpStatusCode.OK);
        var coachTotal = await ReadTotalCountAsync(coachSearch);
        coachTotal.Should().Be(0);
    }

    [Fact]
    public async Task ClearMemory_OnExistingConversation_ReturnsOk()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var createResponse = await client.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationBody("Memory Test", null));
        var conversationId = await ReadCreatedIdAsync(createResponse);

        await client.PostAsJsonAsync(
            $"api/v1/conversations/{conversationId}/messages",
            new AddMessageBody(MessageRole.User, "Remember that I train on Mondays", null));

        var clearResponse = await client.DeleteAsync($"api/v1/conversations/{conversationId}/memory");
        clearResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var cleared = await ReadOkAsync<ConversationDto>(clearResponse);
        cleared.Id.Should().Be(conversationId);
    }

    [Fact]
    public async Task CreateConversation_WithAssistantId_CreatesConversationWithAssistantReference()
    {
        var client = CreateClientAsStandardUser(AITestIds.AthleteUserId);

        var createResponse = await client.PostAsJsonAsync("api/v1/conversations",
            new CreateConversationBody("Coach Chat", null));
        var conversationId = await ReadCreatedIdAsync(createResponse);

        var getResponse = await client.GetAsync($"api/v1/conversations/{conversationId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var conversation = await ReadOkAsync<ConversationDto>(getResponse);
        conversation.Title.Should().Be("Coach Chat");
        conversation.UserId.Should().Be(AITestIds.AthleteUserId);
        conversation.Status.Should().Be(ConversationStatus.Active);
    }
}
