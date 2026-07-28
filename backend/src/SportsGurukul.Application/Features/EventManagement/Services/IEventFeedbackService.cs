using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Services;

public interface IEventFeedbackService
{
    Task<bool> CanSubmitFeedbackAsync(Event evt, Guid userId, CancellationToken cancellationToken = default);
    Task<double> CalculateAverageRatingAsync(Guid eventId, CancellationToken cancellationToken = default);
}
