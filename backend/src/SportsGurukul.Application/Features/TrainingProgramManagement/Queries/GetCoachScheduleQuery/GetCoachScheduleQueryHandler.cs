using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCoachScheduleQuery
{
    public class GetCoachScheduleQueryHandler : IRequestHandler<GetCoachScheduleQuery, Result<IReadOnlyList<TrainingSessionDto>>>
    {
        private readonly ISessionRepository _repository;
        private readonly ILogger<GetCoachScheduleQueryHandler> _logger;

        public GetCoachScheduleQueryHandler(
            ISessionRepository repository,
            ILogger<GetCoachScheduleQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<TrainingSessionDto>>> Handle(GetCoachScheduleQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting schedule for coach: {CoachId} from {StartDate} to {EndDate}",
                request.CoachId, request.StartDate, request.EndDate);

            var sessions = await _repository.GetByCoachIdAsync(request.CoachId, cancellationToken);

            var filteredSessions = sessions
                .Where(s => s.SessionDate >= request.StartDate && s.SessionDate <= request.EndDate)
                .OrderBy(s => s.SessionDate)
                .ThenBy(s => s.StartTime)
                .ToList();

            var dtos = filteredSessions.Select(s => new TrainingSessionDto
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
