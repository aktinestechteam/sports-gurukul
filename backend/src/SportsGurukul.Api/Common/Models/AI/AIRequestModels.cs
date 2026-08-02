using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Common.Models.AI;

public record CreateConversationRequest(string? Title, Guid? AssistantId);
public record RenameConversationRequest(string Title);
public record AddMessageRequest(MessageRole Role, string Content, string? Metadata);
public record RegenerateRequest(Guid MessageId);

public record CreateAssistantRequest(string Name, string? Description, AIAssistantType AssistantType, AIAssistantPersonality Personality, string? SystemPrompt, string? GreetingMessage, bool IsPublic);
public record UpdateAssistantRequest(string? Name, string? Description, AIAssistantType? AssistantType, AIAssistantPersonality? Personality, string? SystemPrompt, string? GreetingMessage, bool? IsPublic);
public record AttachKnowledgeRequest(Guid KnowledgeBaseId);
public record AssignToolsRequest(List<Guid> ToolIds);

public record CreatePromptRequest(string Name, string? Description, PromptType Type, string TemplateContent, string? Variables, string? Tags, string? Category);
public record UpdatePromptRequest(string? Name, string? Description, string? TemplateContent, string? Variables, string? Tags, string? Category);
public record RollbackPromptRequest(int VersionNumber);
public record ClonePromptRequest(string NewName);

public record CreateKnowledgeBaseRequest(string Name, string? Description, KnowledgeBaseVisibility Visibility, string? Category, string? Tags);
public record UpdateKnowledgeBaseRequest(string? Name, string? Description, KnowledgeBaseVisibility? Visibility, string? Category, string? Tags);
public record AttachDocumentRequest(Guid DocumentId);

public record CreateAgentRequest(string Name, string? Description, Guid? AssistantId, string? Configuration, string? Tools, string? Rules, string? Constraints, int? MaxIterations, bool? RequiresApproval);
public record UpdateAgentRequest(string? Name, string? Description, string? Configuration, string? Tools, string? Rules, string? Constraints, int? MaxIterations, bool? RequiresApproval);
public record AssignWorkflowRequest(Guid WorkflowDefinitionId);

public record CreateWorkflowRequest(string Name, string? Description, string? Steps, string? Triggers, string? Conditions, string? Variables);
public record UpdateWorkflowRequest(string? Name, string? Description, string? Steps, string? Triggers, string? Conditions, string? Variables);
