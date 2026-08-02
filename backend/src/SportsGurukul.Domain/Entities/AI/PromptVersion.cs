using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class PromptVersion : BaseEntity
{
    public Guid PromptTemplateId { get; set; }
    public int VersionNumber { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ChangeNotes { get; set; }
    public string? Hash { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public PromptTemplate PromptTemplate { get; set; } = null!;
}
