using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.RescheduleTrainingSession;

public class RescheduleTrainingSessionCommandHandler(
    ILogger<RescheduleTrainingSessionCommandHandler> logger,
    ISessionRepository sessionRepository
) : IRequestHandler<RescheduleTrainingSessionCommand, Result<TrainingSessionDto>>
{
    private readonly ILogger<RescheduleTrainingSessionCommandHandler> _logger = logger;

    public async Task<Result<TrainingSessionDto>> Handle(RescheduleTrainingSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling training session {SessionId}", request.Id);

        var session = await sessionRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (session == null)
        {
            _logger.LogWarning("Training session with ID {SessionId} not found", request.Id);
            return Result<TrainingSessionDto>.Failure($"Training session with ID {request.Id} not found");
        }

        if (session.Status != SessionStatus.Scheduled)
        {
            _logger.LogWarning("Training session {SessionCode} cannot be rescheduled because status is {Status}", session.SessionCode, session.Status);
            return Result<TrainingSessionDto>.Failure("Training session can only be rescheduled when status is Scheduled");
        }

        if (request.StartTime >= request.EndTime)
        {
            _logger.LogWarning("StartTime must be before EndTime");
            return Result<TrainingSessionDto>.Failure("Start time must be before end time");
        }

        var coachSessions = await sessionRepository.GetByCoachIdAsync(session.CoachId, cancellationToken);
        var coachOverlap = coachSessions.Any(s =>
            s.Id != request.Id &&
            s.SessionDate.Date == request.SessionDate.Date &&
            s.StartTime < request.EndTime && s.EndTime > request.StartTime);

        if (coachOverlap)
        {
            _logger.LogWarning("Coach {CoachId} has an overlapping session at the new date and time", session.CoachId);
            return Result<TrainingSessionDto>.Failure("Coach has an overlapping session at the new date and time");
        }

        if (session.FacilityId.HasValue)
        {
            var facilitySessions = await sessionRepository.GetByFacilityIdAsync(session.FacilityId.Value, cancellationToken);
            var facilityOverlap = facilitySessions.Any(s =>
                s.Id != request.Id &&
                s.SessionDate.Date == request.SessionDate.Date &&
                s.StartTime < request.EndTime && s.EndTime > request.StartTime);

            if (facilityOverlap)
            {
                _logger.LogWarning("Facility {FacilityId} is already booked for the new date and time", session.FacilityId);
                return Result<TrainingSessionDto>.Failure("Facility is already booked for the new date and time");
            }
        }

        session.SessionDate = request.SessionDate;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.UpdatedAt = DateTime.UtcNow;

        sessionRepository.Update(session);

        _logger.LogInformation("Training session {SessionCode} rescheduled successfully", session.SessionCode);

        var rescheduledSession = await sessionRepository.GetByIdWithDetailsAsync(session.Id, cancellationToken);
        return Result<TrainingSessionDto>.Success(MapToDto(rescheduledSession!));
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
