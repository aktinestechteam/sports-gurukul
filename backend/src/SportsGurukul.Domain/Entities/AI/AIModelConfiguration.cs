using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIModelConfiguration : BaseEntity
{
    public Guid ModelId { get; set; }
    public string? DisplayName { get; set; }
    public double? Temperature { get; set; }
    public int? MaxTokens { get; set; }
    public double? TopP { get; set; }
    public double? FrequencyPenalty { get; set; }
    public double? PresencePenalty { get; set; }
    public string? StopSequences { get; set; }
    public string? ModelParameters { get; set; }
    public bool IsDefault { get; set; } = false;
    public byte[] RowVersion { get; set; } = [];

    public AIModel Model { get; set; } = null!;
}
