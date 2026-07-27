using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public sealed record SchedulingResult
{
    public bool IsSuccess { get; init; }
    public IReadOnlyList<TimeSlot> GeneratedSlots { get; init; } = [];
    public IReadOnlyList<ConflictInfo> Conflicts { get; init; } = [];
    public IReadOnlyList<TimeSlot> Alternatives { get; init; } = [];
    public string? Message { get; init; }
    public TimeSpan ComputationTime { get; init; }
    
    public static SchedulingResult Success(IReadOnlyList<TimeSlot> slots, TimeSpan computation) => new() { IsSuccess = true, GeneratedSlots = slots, ComputationTime = computation };
    public static SchedulingResult Failure(string message, IReadOnlyList<ConflictInfo>? conflicts = null, TimeSpan computation = default) => new() { IsSuccess = false, Message = message, Conflicts = conflicts ?? [], ComputationTime = computation };
}
