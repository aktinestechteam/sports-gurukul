using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RemoveFromWaitlist;

public class RemoveFromWaitlistCommand : IRequest<Result<bool>>
{
    public Guid WaitlistEntryId { get; set; }
}
