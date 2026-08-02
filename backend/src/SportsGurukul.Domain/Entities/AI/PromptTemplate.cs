using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Domain.Entities.AI;

public class PromptTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public PromptType Type { get; set; } = PromptType.Template;
    public PromptStatus Status { get; set; } = PromptStatus.Draft;
    public string TemplateContent { get; set; } = string.Empty;
    public string? Variables { get; set; }
    public string? Tags { get; set; }
    public int CurrentVersion { get; set; } = 1;
    public string? Category { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public ICollection<PromptVersion>? Versions { get; set; }
}
