using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingMaterial : BaseEntity
{
    public Guid ProgramId { get; set; }
    public Guid? SessionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MaterialType MaterialType { get; set; } = MaterialType.Document;
    public string FileUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public TrainingProgram Program { get; set; } = null!;
    public TrainingSession? Session { get; set; }
}
