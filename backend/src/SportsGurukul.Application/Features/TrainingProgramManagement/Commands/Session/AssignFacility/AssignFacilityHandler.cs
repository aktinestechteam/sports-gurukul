using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.AssignFacility;

public class AssignFacilityCommandHandler(
    ILogger<AssignFacilityCommandHandler> logger,
    ISessionRepository sessionRepository,
    IFacilityRepository facilityRepository
) : IRequestHandler<AssignFacilityCommand, Result<TrainingSessionDto>>
{
    private readonly ILogger<AssignFacilityCommandHandler> _logger = logger;

    public async Task<Result<TrainingSessionDto>> Handle(AssignFacilityCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning facility to training session {SessionId}", request.Id);

        var session = await sessionRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (session == null)
        {
            _logger.LogWarning("Training session with ID {SessionId} not found", request.Id);
            return Result<TrainingSessionDto>.Failure($"Training session with ID {request.Id} not found");
        }

        if (request.FacilityId.HasValue)
        {
            var facility = await facilityRepository.GetByIdAsync(request.FacilityId.Value, cancellationToken);
            if (facility == null)
            {
                _logger.LogWarning("Facility with ID {FacilityId} not found", request.FacilityId);
                return Result<TrainingSessionDto>.Failure($"Facility with ID {request.FacilityId} not found");
            }

            var facilitySessions = await sessionRepository.GetByFacilityIdAsync(request.FacilityId.Value, cancellationToken);
            var doubleBooked = facilitySessions.Any(s =>
                s.Id != request.Id &&
                s.SessionDate.Date == session.SessionDate.Date &&
                s.StartTime < session.EndTime && s.EndTime > session.StartTime);

            if (doubleBooked)
            {
                _logger.LogWarning("Facility {FacilityId} is already booked for {SessionDate}", request.FacilityId, session.SessionDate);
                return Result<TrainingSessionDto>.Failure("Facility is already booked for the specified date and time");
            }
        }

        session.FacilityId = request.FacilityId;
        session.UpdatedAt = DateTime.UtcNow;

        sessionRepository.Update(session);

        _logger.LogInformation("Facility assigned to session {SessionCode} successfully", session.SessionCode);

        var updatedSession = await sessionRepository.GetByIdWithDetailsAsync(session.Id, cancellationToken);
        return Result<TrainingSessionDto>.Success(MapToDto(updatedSession!));
    }

    public static TrainingSessionDto MapToDto(TrainingSession session) => new()
    {
        Id = session.Id,
        BatchId = session.BatchId,
        BatchCode = session.Batch?.BatchCode ?? string.Empty,
        SessionCode = session.SessionCode,
        SessionTitle = session.SessionTitle,
        SessionType = session.SessionType.ToString(),
        SessionDate = session.SessionDate,
        StartTime = session.StartTime,
        EndTime = session.EndTime,
        FacilityId = session.FacilityId,
        FacilityName = session.Facility?.FacilityName,
        CoachId = session.CoachId,
        CoachName = session.Coach?.User?.FullName ?? string.Empty,
        Status = session.Status.ToString(),
        AttendanceCount = session.Attendances?.Count ?? 0,
        RowVersion = session.RowVersion,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt
    };
}
