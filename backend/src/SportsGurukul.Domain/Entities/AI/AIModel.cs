using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIModel : BaseEntity
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public AIModelCapability Capabilities { get; set; }
    public AIModelStatus Status { get; set; } = AIModelStatus.Active;
    public int? MaxTokens { get; set; }
    public int? MaxContextLength { get; set; }
    public decimal? CostPerInputToken { get; set; }
    public decimal? CostPerOutputToken { get; set; }
    public decimal? CostPerImageToken { get; set; }
    public double? TemperatureMin { get; set; }
    public double? TemperatureMax { get; set; }
    public double DefaultTemperature { get; set; } = 0.7;
    public bool SupportsStreaming { get; set; } = true;
    public bool SupportsFunctionCalling { get; set; } = false;
    public bool SupportsVision { get; set; } = false;
    public bool SupportsEmbeddings { get; set; } = false;
    public string? ModelVersion { get; set; }
    public DateTime? ReleasedAt { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AIProvider Provider { get; set; } = null!;
    public ICollection<AIModelConfiguration>? ModelConfigurations { get; set; }
}
