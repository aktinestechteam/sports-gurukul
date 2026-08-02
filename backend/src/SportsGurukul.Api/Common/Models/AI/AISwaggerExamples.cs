using SportsGurukul.Domain.Enums.AI;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.AI;

public class CreateConversationRequestExample : IExamplesProvider<CreateConversationRequest>
{
    public CreateConversationRequest GetExamples() => new("Squad Selection Discussion", Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));
}

public class RenameConversationRequestExample : IExamplesProvider<RenameConversationRequest>
{
    public RenameConversationRequest GetExamples() => new("Updated: Squad Selection Strategy");
}

public class AddMessageRequestExample : IExamplesProvider<AddMessageRequest>
{
    public AddMessageRequest GetExamples() => new(MessageRole.User, "Which players are eligible for the U-19 tournament?", null);
}

public class RegenerateRequestExample : IExamplesProvider<RegenerateRequest>
{
    public RegenerateRequest GetExamples() => new(Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"));
}

public class CreateAssistantRequestExample : IExamplesProvider<CreateAssistantRequest>
{
    public CreateAssistantRequest GetExamples() => new(
        "Cricket Coach AI",
        "AI assistant for cricket coaching and strategy",
        AIAssistantType.Coach,
        AIAssistantPersonality.Motivational,
        "You are a professional cricket coach...",
        "Hello! I am your cricket coaching assistant.",
        true);
}

public class UpdateAssistantRequestExample : IExamplesProvider<UpdateAssistantRequest>
{
    public UpdateAssistantRequest GetExamples() => new(
        "Cricket Coach AI Pro",
        "Updated description for advanced coaching",
        AIAssistantType.Coach,
        AIAssistantPersonality.Analytical,
        "You are an expert cricket analyst...",
        "Welcome! I am your advanced coaching assistant.",
        true);
}

public class AttachKnowledgeRequestExample : IExamplesProvider<AttachKnowledgeRequest>
{
    public AttachKnowledgeRequest GetExamples() => new(Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"));
}

public class AssignToolsRequestExample : IExamplesProvider<AssignToolsRequest>
{
    public AssignToolsRequest GetExamples() => new(new List<Guid> { Guid.Parse("d4e5f6a7-b8c9-0123-defa-123456789013") });
}

public class CreatePromptRequestExample : IExamplesProvider<CreatePromptRequest>
{
    public CreatePromptRequest GetExamples() => new(
        "Match Analysis Prompt",
        "Used for post-match analysis",
        PromptType.Template,
        "Analyze the match between {team1} and {team2}. Focus on {aspect}.",
        "team1,team2,aspect",
        "analysis,match",
        "Cricket");
}

public class UpdatePromptRequestExample : IExamplesProvider<UpdatePromptRequest>
{
    public UpdatePromptRequest GetExamples() => new(
        "Match Analysis Prompt V2",
        "Updated for detailed analysis",
        "Analyze {team1} vs {team2} comprehensively. Cover {aspect} in depth.",
        "team1,team2,aspect",
        "analysis,cricket",
        "Cricket");
}

public class RollbackPromptRequestExample : IExamplesProvider<RollbackPromptRequest>
{
    public RollbackPromptRequest GetExamples() => new(1);
}

public class ClonePromptRequestExample : IExamplesProvider<ClonePromptRequest>
{
    public ClonePromptRequest GetExamples() => new("Match Analysis Prompt (Copy)");
}

public class CreateKnowledgeBaseRequestExample : IExamplesProvider<CreateKnowledgeBaseRequest>
{
    public CreateKnowledgeBaseRequest GetExamples() => new(
        "Cricket Rules & Regulations",
        "Official ICC cricket rules and regulations",
        KnowledgeBaseVisibility.Public,
        "Sports",
        "cricket,rules,icc");
}

public class UpdateKnowledgeBaseRequestExample : IExamplesProvider<UpdateKnowledgeBaseRequest>
{
    public UpdateKnowledgeBaseRequest GetExamples() => new(
        "Cricket Rules & Regulations 2025",
        "Updated ICC rules for 2025 season",
        KnowledgeBaseVisibility.Public,
        "Sports",
        "cricket,rules,icc,2025");
}

public class CreateAgentRequestExample : IExamplesProvider<CreateAgentRequest>
{
    public CreateAgentRequest GetExamples() => new(
        "Match Scheduler Agent",
        "Automates match scheduling based on team availability",
        Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        "{\"timezone\":\"UTC\",\"defaultDuration\":120}",
        "[\"calendar\",\"notification\"]",
        "Schedule matches only during daylight hours",
        "Max 4 matches per day",
        10,
        true);
}

public class UpdateAgentRequestExample : IExamplesProvider<UpdateAgentRequest>
{
    public UpdateAgentRequest GetExamples() => new(
        "Match Scheduler Agent Pro",
        "Enhanced scheduling with venue optimization",
        "{\"timezone\":\"UTC\",\"defaultDuration\":90,\"optimizeVenues\":true}",
        "[\"calendar\",\"notification\",\"venue\"]",
        "Schedule matches during daylight hours only; prefer home venue",
        "Max 3 matches per day per venue",
        15,
        true);
}

public class AssignWorkflowRequestExample : IExamplesProvider<AssignWorkflowRequest>
{
    public AssignWorkflowRequest GetExamples() => new(Guid.Parse("e5f6a7b8-c9d0-1234-efab-123456789014"));
}

public class CreateWorkflowRequestExample : IExamplesProvider<CreateWorkflowRequest>
{
    public CreateWorkflowRequest GetExamples() => new(
        "Tournament Registration Workflow",
        "Handles end-to-end tournament registration",
        "[{\"step\":\"validate\",\"action\":\"checkEligibility\"},{\"step\":\"payment\",\"action\":\"processFee\"},{\"step\":\"confirm\",\"action\":\"sendConfirmation\"}]",
        "[\"onRegistrationSubmit\",\"onPaymentComplete\"]",
        "{\"minAge\":10,\"maxAge\":19,\"requireGuardian\":true}",
        "{\"registrationFee\":500,\"currency\":\"INR\"}");
}

public class UpdateWorkflowRequestExample : IExamplesProvider<UpdateWorkflowRequest>
{
    public UpdateWorkflowRequest GetExamples() => new(
        "Tournament Registration Workflow V2",
        "Updated with team registration support",
        "[{\"step\":\"validate\",\"action\":\"checkEligibility\"},{\"step\":\"teamValidate\",\"action\":\"validateTeamComposition\"},{\"step\":\"payment\",\"action\":\"processFee\"},{\"step\":\"confirm\",\"action\":\"sendConfirmation\"}]",
        "[\"onRegistrationSubmit\",\"onTeamSubmit\",\"onPaymentComplete\"]",
        "{\"minAge\":10,\"maxAge\":19,\"requireGuardian\":true,\"maxTeamSize\":15}",
        "{\"registrationFee\":500,\"teamFee\":2500,\"currency\":\"INR\"}");
}
