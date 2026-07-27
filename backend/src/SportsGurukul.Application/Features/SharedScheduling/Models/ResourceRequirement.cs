namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record ResourceRequirement
{
    public string ResourceType { get; init; } = string.Empty;
    public Guid ResourceId { get; init; }
    public bool IsRequired { get; init; } = true;
    public string? ResourceName { get; init; }
}
