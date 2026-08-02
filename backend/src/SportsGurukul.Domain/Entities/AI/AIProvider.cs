using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AIProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AIProviderType Type { get; set; }
    public string? ApiBaseUrl { get; set; }
    public string? ApiVersion { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxRetries { get; set; }
    public int? TimeoutSeconds { get; set; }
    public decimal? CostPerToken { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<AIModel> Models { get; set; } = new List<AIModel>();
}
