namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record Holiday
{
    public DateTime Date { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool IsRecurring { get; init; }
    public Guid? AcademyId { get; init; }
}
