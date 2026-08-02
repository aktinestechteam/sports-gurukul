using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class AgentExecution : BaseEntity
{
    public Guid AgentDefinitionId { get; set; }
    public AgentExecutionStatus Status { get; set; } = AgentExecutionStatus.Pending;
    public string? Input { get; set; }
    public string? Output { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int? Iterations { get; set; }
    public int? TokensUsed { get; set; }
    public decimal? Cost { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public AgentDefinition AgentDefinition { get; set; } = null!;
}
