using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class ToolExecution : BaseEntity
{
    public Guid ToolDefinitionId { get; set; }
    public Guid? ConversationId { get; set; }
    public string? Input { get; set; }
    public string? Output { get; set; }
    public bool IsSuccess { get; set; } = false;
    public string? ErrorMessage { get; set; }
    public double? ExecutionTimeMs { get; set; }
    public decimal? Cost { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ToolDefinition ToolDefinition { get; set; } = null!;
    public Conversation? Conversation { get; set; }
}
