using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Commands.ValidateBookingSlot;

public class ValidateBookingSlotCommand : IRequest<Result<bool>>
{
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public IReadOnlyList<ResourceRequirement> Resources { get; set; } = [];
    public string? TimeZoneId { get; set; }
}
