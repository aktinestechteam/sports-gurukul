using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetFeedbackByEvent;

public class GetFeedbackByEventQuery : IRequest<Result<List<FeedbackDto>>>
{
    public Guid EventId { get; set; }
}
