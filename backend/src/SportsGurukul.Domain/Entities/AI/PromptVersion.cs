using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities.AI;

public class PromptVersion : BaseEntity
{
    public Guid PromptTemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ChangeSummary { get; set; }
    public string? Notes { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeployedAt { get; set; }
    public string? EvaluationJson { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public PromptTemplate? PromptTemplate { get; set; }
}
