using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.RejectFeedback;

public class RejectFeedbackCommand : IRequest<Result<FeedbackDto>>
{
    public Guid FeedbackId { get; set; }
    public string? Reason { get; set; }
}
