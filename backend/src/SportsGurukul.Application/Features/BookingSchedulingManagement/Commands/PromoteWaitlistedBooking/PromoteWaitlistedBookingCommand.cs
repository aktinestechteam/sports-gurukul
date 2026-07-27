using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.PromoteWaitlistedBooking;

public class PromoteWaitlistedBookingCommand : IRequest<Result<BookingDto>>
{
    public Guid WaitlistEntryId { get; set; }
}
