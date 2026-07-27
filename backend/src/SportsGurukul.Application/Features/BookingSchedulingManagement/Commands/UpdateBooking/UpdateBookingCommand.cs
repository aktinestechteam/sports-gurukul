using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.UpdateBooking;

public class UpdateBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? BookingDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
}
