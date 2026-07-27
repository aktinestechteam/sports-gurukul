using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetUpcomingSessionsQuery
{
    public class GetUpcomingSessionsQueryHandler : IRequestHandler<GetUpcomingSessionsQuery, Result<IReadOnlyList<TrainingSessionDto>>>
    {
        private readonly ISessionRepository _repository;
        private readonly ILogger<GetUpcomingSessionsQueryHandler> _logger;

        public GetUpcomingSessionsQueryHandler(
            ISessionRepository repository,
            ILogger<GetUpcomingSessionsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<TrainingSessionDto>>> Handle(GetUpcomingSessionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting upcoming sessions for CoachId: {CoachId}, BatchId: {BatchId}", request.CoachId, request.BatchId);

            IEnumerable<Domain.Entities.TrainingSession> sessions;

            if (request.BatchId.HasValue)
            {
                sessions = await _repository.GetByBatchIdAsync(request.BatchId.Value, cancellationToken);
            }
            else if (request.CoachId.HasValue)
            {
                sessions = await _repository.GetByCoachIdAsync(request.CoachId.Value, cancellationToken);
            }
            else
            {
                sessions = await _repository.GetAllAsync(cancellationToken);
            }

            var now = DateTime.UtcNow;
            var upcomingSessions = sessions
                .Where(s => s.SessionDate >= now.Date)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime);

            var dtos = upcomingSessions.Select(TrainingSessionDto.MapToDto).ToList();

            return Result<IReadOnlyList<TrainingSessionDto>>.Success(dtos);
        }
    }
}
