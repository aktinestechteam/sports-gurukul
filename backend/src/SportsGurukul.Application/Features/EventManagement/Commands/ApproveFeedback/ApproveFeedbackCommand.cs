using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ApproveFeedback;

public class ApproveFeedbackCommand : IRequest<Result<FeedbackDto>>
{
    public Guid FeedbackId { get; set; }
}
