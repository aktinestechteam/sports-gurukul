using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.RejectBookingApproval;

public class RejectBookingApprovalCommand : IRequest<Result<BookingDto>>
{
    public Guid BookingId { get; set; }
    public Guid ApproverUserId { get; set; }
    public string? Comments { get; set; }
}
