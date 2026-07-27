using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.CreateRecurringBooking;

public class CreateRecurringBookingCommand : IRequest<Result<BookingDto>>
{
    public string BookingType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? TrainingSessionId { get; set; }
    public DateTime StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string RecurrenceType { get; set; } = string.Empty;
    public int? OccurrenceCount { get; set; }
    public DateTime? EndDate { get; set; }
    public string? RRule { get; set; }
    public string? Exceptions { get; set; }
}
