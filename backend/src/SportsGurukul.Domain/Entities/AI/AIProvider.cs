using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIProviderType ProviderType { get; set; }
    public string? BaseUrl { get; set; }
    public AIAuthType AuthType { get; set; } = AIAuthType.ApiKey;
    public string? DefaultApiVersion { get; set; }
    public bool SupportsChat { get; set; }
    public bool SupportsEmbeddings { get; set; }
    public bool SupportsVision { get; set; }
    public bool SupportsFunctionCalling { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ConfigurationSchemaJson { get; set; }
    public string? IconUrl { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? DocumentationUrl { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<AIModel> Models { get; set; } = new List<AIModel>();
    public ICollection<AIModelConfiguration> ModelConfigurations { get; set; } = new List<AIModelConfiguration>();
    public ICollection<AITokenUsage> TokenUsages { get; set; } = new List<AITokenUsage>();
    public ICollection<AIRoutingPolicy> RoutingPolicies { get; set; } = new List<AIRoutingPolicy>();
}
