using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.AI;

public class AIModelConfiguration : BaseEntity
{
    public Guid? ProviderId { get; set; }
    public Guid? ModelId { get; set; }
    public Guid? AssistantId { get; set; }
    public Guid? AgentDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double? Temperature { get; set; }
    public double? TopP { get; set; }
    public double? TopK { get; set; }
    public int? MaxTokens { get; set; }
    public string? StopSequencesJson { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public string? ApiKeyEncrypted { get; set; }
    public string? ApiVersion { get; set; }
    public string? BaseUrlOverride { get; set; }
    public int? TimeoutSeconds { get; set; }
    public int? MaxRetries { get; set; }
    public bool StreamingEnabled { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public AIProvider? Provider { get; set; }
    public AIModel? Model { get; set; }
    public AIAssistant? Assistant { get; set; }
    public AgentDefinition? AgentDefinition { get; set; }
}
