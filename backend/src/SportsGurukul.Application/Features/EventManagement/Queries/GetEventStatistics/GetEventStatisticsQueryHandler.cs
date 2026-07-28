using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetEventStatistics;

public class GetEventStatisticsQueryHandler : IRequestHandler<GetEventStatisticsQuery, Result<StatisticsDto>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<GetEventStatisticsQueryHandler> _logger;

    public GetEventStatisticsQueryHandler(
        IEventRepository eventRepository,
        ILogger<GetEventStatisticsQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<StatisticsDto>> Handle(GetEventStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting statistics for event: {EventId}", request.EventId);

        var evt = await _eventRepository.GetWithDetailsAsync(request.EventId, cancellationToken);
        if (evt is null)
        {
            _logger.LogWarning("Event not found: {EventId}", request.EventId);
            return Result<StatisticsDto>.Failure("Event not found.");
        }

        var registrations = evt.Registrations?.ToList() ?? [];
        var stats = new StatisticsDto
        {
            EventId = evt.Id,
            EventName = evt.EventName,
            Status = evt.Status.ToString(),
            TotalRegistrations = registrations.Count,
            ApprovedRegistrations = registrations.Count(r => r.Status == EventRegistrationStatus.Approved),
            PendingRegistrations = registrations.Count(r => r.Status == EventRegistrationStatus.Pending),
            CancelledRegistrations = registrations.Count(r => r.Status == EventRegistrationStatus.Cancelled),
            WaitlistedRegistrations = registrations.Count(r => r.Status == EventRegistrationStatus.Waitlisted),
            TotalParticipants = evt.Participants?.Count ?? 0,
            TotalSessions = evt.Sessions?.Count ?? 0,
            CompletedSessions = evt.Sessions?.Count(s => s.Status == EventSessionStatus.Completed) ?? 0,
            CertificatesIssued = evt.Certificates?.Count ?? 0,
            FeedbackCount = evt.Feedbacks?.Count ?? 0,
            AverageFeedbackScore = evt.Feedbacks?.Any() == true ? evt.Feedbacks.Average(f => (double)f.OverallRating) : 0
        };

        return Result<StatisticsDto>.Success(stats);
    }
}
