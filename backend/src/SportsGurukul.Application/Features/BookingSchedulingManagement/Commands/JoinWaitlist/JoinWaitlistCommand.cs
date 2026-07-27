using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.JoinWaitlist;

public class JoinWaitlistCommand : IRequest<Result<WaitlistDto>>
{
    public Guid BookingId { get; set; }
    public Guid WaitlistUserId { get; set; }
    public string? Notes { get; set; }
}
