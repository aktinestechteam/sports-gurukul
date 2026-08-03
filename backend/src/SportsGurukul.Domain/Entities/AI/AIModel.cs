using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIModel : BaseEntity
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public AIModelFamily Family { get; set; }
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public int? ContextWindow { get; set; }
    public int? MaxOutputTokens { get; set; }
    public decimal? InputCostPerMillionTokens { get; set; }
    public decimal? OutputCostPerMillionTokens { get; set; }
    public string Currency { get; set; } = "USD";
    public bool SupportsChat { get; set; }
    public bool SupportsEmbeddings { get; set; }
    public bool SupportsVision { get; set; }
    public bool SupportsFunctionCalling { get; set; }
    public bool SupportsJsonMode { get; set; }
    public bool SupportsStreaming { get; set; }
    public bool IsActive { get; set; } = true;
    public int? RateLimitPerMinute { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIProvider? Provider { get; set; }
    public ICollection<Embedding> Embeddings { get; set; } = new List<Embedding>();
    public ICollection<AIModelConfiguration> ModelConfigurations { get; set; } = new List<AIModelConfiguration>();
    public ICollection<AITokenUsage> TokenUsages { get; set; } = new List<AITokenUsage>();
    public ICollection<AIRoutingPolicy> DefaultRoutingPolicies { get; set; } = new List<AIRoutingPolicy>();
}
