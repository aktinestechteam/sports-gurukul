using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;

public class UpdateAvailabilityCommand : IRequest<Result<AvailabilityDto>>
{
    public Guid CoachId { get; set; }
    public string? WeeklySchedule { get; set; }
    public string? TimeSlots { get; set; }
    public bool? OnlineAvailable { get; set; }
    public bool? OfflineAvailable { get; set; }
    public int? TravelDistance { get; set; }
}
