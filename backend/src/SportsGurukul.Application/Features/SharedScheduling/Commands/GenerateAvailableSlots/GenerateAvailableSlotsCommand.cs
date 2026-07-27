using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.GenerateAvailableSlots;

public class GenerateAvailableSlotsCommand : IRequest<Result<IReadOnlyList<TimeSlot>>>
{
    public Guid ResourceId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public TimeSpan? SlotDuration { get; set; }
    public string? TimeZoneId { get; set; }
}
