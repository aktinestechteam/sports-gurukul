using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.SubmitFeedback;

public class SubmitFeedbackCommand : IRequest<Result<FeedbackDto>>
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public int OverallRating { get; set; }
    public int? ContentRating { get; set; }
    public int? SpeakerRating { get; set; }
    public int? VenueRating { get; set; }
    public int? OrganizationRating { get; set; }
    public string? Comments { get; set; }
    public string? Suggestions { get; set; }
    public bool WouldRecommend { get; set; }
    public bool IsAnonymous { get; set; }
}
