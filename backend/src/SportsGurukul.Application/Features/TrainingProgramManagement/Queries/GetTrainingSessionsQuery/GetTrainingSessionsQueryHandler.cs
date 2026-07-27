using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetTrainingSessionsQuery
{
    public class GetTrainingSessionsQueryHandler : IRequestHandler<GetTrainingSessionsQuery, Result<IReadOnlyList<TrainingSessionDto>>>
    {
        private readonly ISessionRepository _repository;
        private readonly ILogger<GetTrainingSessionsQueryHandler> _logger;

        public GetTrainingSessionsQueryHandler(
            ISessionRepository repository,
            ILogger<GetTrainingSessionsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<TrainingSessionDto>>> Handle(GetTrainingSessionsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting training sessions for BatchId: {BatchId}, DateRange: {StartDate} - {EndDate}",
                request.BatchId, request.StartDate, request.EndDate);

            IEnumerable<Domain.Entities.TrainingSession> sessions;

            if (request.BatchId.HasValue)
            {
                sessions = await _repository.GetByBatchIdAsync(request.BatchId.Value, cancellationToken);
            }
            else if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var allSessions = await _repository.GetAllAsync(cancellationToken);
                sessions = allSessions.Where(s => s.SessionDate >= request.StartDate.Value && s.SessionDate <= request.EndDate.Value);
            }
            else
            {
                sessions = await _repository.GetAllAsync(cancellationToken);
            }

            var dtos = sessions.Select(s => new TrainingSessionDto
            {
                Id = s.Id,
                BatchId = s.BatchId,
                BatchCode = s.Batch?.BatchCode ?? string.Empty,
                SessionCode = s.SessionCode,
                SessionTitle = s.SessionTitle,
                SessionType = s.SessionType.ToString(),
                SessionDate = s.SessionDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                FacilityId = s.FacilityId,
                FacilityName = s.Facility?.FacilityName,
                CoachId = s.CoachId,
                CoachName = s.Coach?.User?.FullName ?? string.Empty,
                Status = s.Status.ToString(),
                AttendanceCount = s.Attendances?.Count ?? 0,
                RowVersion = s.RowVersion ?? [],
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Result<IReadOnlyList<TrainingSessionDto>>.Success(dtos);
        }
    }
}
