using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.OptimizeSchedule;

public class OptimizeScheduleCommand : IRequest<Result<TimeSlot?>>
{
    public string ResourceType { get; set; } = string.Empty;
    public IReadOnlyList<Guid> ResourceIds { get; set; } = [];
    public DateTime PreferredDate { get; set; }
    public TimeSpan Duration { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public string? TimeZoneId { get; set; }
}
