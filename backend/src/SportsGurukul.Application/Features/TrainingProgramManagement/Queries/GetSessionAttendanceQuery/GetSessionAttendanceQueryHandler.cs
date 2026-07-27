using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetSessionAttendanceQuery
{
    public class GetSessionAttendanceQueryHandler : IRequestHandler<GetSessionAttendanceQuery, Result<IReadOnlyList<AttendanceDto>>>
    {
        private readonly IAttendanceRepository _repository;
        private readonly ILogger<GetSessionAttendanceQueryHandler> _logger;

        public GetSessionAttendanceQueryHandler(
            IAttendanceRepository repository,
            ILogger<GetSessionAttendanceQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<AttendanceDto>>> Handle(GetSessionAttendanceQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting attendance for session: {SessionId}", request.SessionId);

            var attendances = await _repository.GetBySessionIdAsync(request.SessionId, cancellationToken);

            var dtos = attendances.Select(a => new AttendanceDto
            {
                Id = a.Id,
                SessionId = a.SessionId,
                SessionCode = a.Session?.SessionCode ?? string.Empty,
                AthleteId = a.AthleteId,
                AthleteName = a.Athlete?.User?.FullName ?? string.Empty,
                AthleteCode = a.Athlete?.AthleteCode ?? string.Empty,
                AttendanceStatus = a.AttendanceStatus.ToString(),
                CheckInTime = a.CheckInTime,
                CheckOutTime = a.CheckOutTime,
                Remarks = a.Remarks,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            return Result<IReadOnlyList<AttendanceDto>>.Success(dtos);
        }
    }
}
