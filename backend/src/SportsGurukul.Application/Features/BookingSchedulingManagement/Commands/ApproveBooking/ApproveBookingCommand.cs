using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ApproveBooking;

public class ApproveBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public Guid ApproverUserId { get; set; }
    public string? Comments { get; set; }
}
