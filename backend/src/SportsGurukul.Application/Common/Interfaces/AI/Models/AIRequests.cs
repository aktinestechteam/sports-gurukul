using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Models;

public record CreateConversationRequest(string? Title, Guid? AssistantId, Guid? UserId);
public record AddMessageRequest(Guid ConversationId, MessageRole Role, string Content, string? Metadata);
public record SendMessageRequest(Guid ConversationId, string Message, string? ModelName, double? Temperature);
public record GetCompletionRequest(string Prompt, string? ModelName, double? Temperature, int? MaxTokens);
public record CreateAssistantRequest(string Name, string? Description, AIAssistantType AssistantType, AIAssistantPersonality Personality, string? SystemPrompt, string? GreetingMessage, bool IsPublic);
public record UpdateAssistantRequest(Guid Id, string? Name, string? Description, AIAssistantType? AssistantType, AIAssistantPersonality? Personality, string? SystemPrompt, string? GreetingMessage, bool? IsPublic);
public record CreatePromptTemplateRequest(string Name, string? Description, PromptType Type, string TemplateContent, string? Variables, string? Tags, string? Category);
public record UpdatePromptTemplateRequest(Guid Id, string? Name, string? Description, string? TemplateContent, string? Variables, string? Tags, string? Category);
public record CreateKnowledgeBaseRequest(string Name, string? Description, KnowledgeBaseVisibility Visibility, string? Category, string? Tags);
public record UpdateKnowledgeBaseRequest(Guid Id, string? Name, string? Description, KnowledgeBaseVisibility? Visibility, string? Category, string? Tags);
public record CreateAgentRequest(string Name, string? Description, Guid? AssistantId, string? Configuration, string? Tools, string? Rules, string? Constraints, int? MaxIterations, bool? RequiresApproval);
public record UpdateAgentRequest(Guid Id, string? Name, string? Description, string? Configuration, string? Tools, string? Rules, string? Constraints, int? MaxIterations, bool? RequiresApproval);
public record RecordTokenUsageRequest(Guid? ConversationId, Guid? MessageId, string ModelName, string? ProviderName, int PromptTokens, int CompletionTokens, int TotalTokens, decimal? Cost, string? UserId, string? SessionId, string? RequestType);
public record RecordAuditRequest(Guid? EntityId, string EntityType, AuditEventType EventType, AuditSeverity Severity, string? Action, string? ActorId, string? ActorType, string? IpAddress, string? UserAgent, string? PreviousState, string? NewState, string? Message, string? Metadata);
public record SearchConversationsRequest(string? SearchTerm, Guid? AssistantId, Guid? UserId, ConversationStatus? Status, DateTime? FromDate, DateTime? ToDate, int Page = 1, int PageSize = 20);
public record SearchAssistantsRequest(string? SearchTerm, AIAssistantType? AssistantType, bool? IsActive, bool? IsPublic, int Page = 1, int PageSize = 20);
public record SearchPromptsRequest(string? SearchTerm, PromptType? Type, PromptStatus? Status, string? Category, int Page = 1, int PageSize = 20);
public record SearchKnowledgeBasesRequest(string? SearchTerm, KnowledgeBaseVisibility? Visibility, KnowledgeBaseStatus? Status, string? Category, int Page = 1, int PageSize = 20);
public record SearchAgentsRequest(string? SearchTerm, AgentStatus? Status, Guid? AssistantId, int Page = 1, int PageSize = 20);
public record SearchWorkflowsRequest(string? SearchTerm, WorkflowStatus? Status, int Page = 1, int PageSize = 20);
public record SearchTokenUsageRequest(string? ModelName, string? UserId, DateTime? FromDate, DateTime? ToDate, int Page = 1, int PageSize = 20);
public record SearchAuditRequest(string? EntityType, Guid? EntityId, AuditEventType? EventType, AuditSeverity? Severity, DateTime? FromDate, DateTime? ToDate, int Page = 1, int PageSize = 20);
