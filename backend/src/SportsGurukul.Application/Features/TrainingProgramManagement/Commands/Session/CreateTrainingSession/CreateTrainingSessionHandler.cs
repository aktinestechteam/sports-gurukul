using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CreateTrainingSession;

public class CreateTrainingSessionCommandHandler(
    ILogger<CreateTrainingSessionCommandHandler> logger,
    ITrainingBatchRepository batchRepository,
    ISessionRepository sessionRepository,
    ICoachRepository coachRepository,
    IFacilityRepository facilityRepository
) : IRequestHandler<CreateTrainingSessionCommand, Result<TrainingSessionDto>>
{
    private readonly ILogger<CreateTrainingSessionCommandHandler> _logger = logger;

    public async Task<Result<TrainingSessionDto>> Handle(CreateTrainingSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training session for batch {BatchId}", request.BatchId);

        var batch = await batchRepository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Training batch with ID {BatchId} not found", request.BatchId);
            return Result<TrainingSessionDto>.Failure($"Training batch with ID {request.BatchId} not found");
        }

        if (batch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Training session cannot be created for batch {BatchCode} because status is {Status}", batch.BatchCode, batch.Status);
            return Result<TrainingSessionDto>.Failure("Training sessions can only be created for Active batches");
        }

        var coach = await coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach == null)
        {
            _logger.LogWarning("Coach with ID {CoachId} not found", request.CoachId);
            return Result<TrainingSessionDto>.Failure($"Coach with ID {request.CoachId} not found");
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
            var facilityOverlap = facilitySessions.Any(s =>
                s.SessionDate.Date == request.SessionDate.Date &&
                s.StartTime < request.EndTime && s.EndTime > request.StartTime);

            if (facilityOverlap)
            {
                _logger.LogWarning("Facility {FacilityId} is already booked for the specified date and time", request.FacilityId);
                return Result<TrainingSessionDto>.Failure("Facility is already booked for the specified date and time");
            }
        }

        var coachSessions = await sessionRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);
        var coachOverlap = coachSessions.Any(s =>
            s.SessionDate.Date == request.SessionDate.Date &&
            s.StartTime < request.EndTime && s.EndTime > request.StartTime);

        if (coachOverlap)
        {
            _logger.LogWarning("Coach {CoachId} has an overlapping session at the specified date and time", request.CoachId);
            return Result<TrainingSessionDto>.Failure("Coach has an overlapping session at the specified date and time");
        }

        if (request.StartTime >= request.EndTime)
        {
            _logger.LogWarning("StartTime must be before EndTime");
            return Result<TrainingSessionDto>.Failure("Start time must be before end time");
        }

        string sessionCode;
        bool isUnique;
        do
        {
            sessionCode = $"SES-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
            isUnique = await sessionRepository.IsSessionCodeUniqueAsync(sessionCode, cancellationToken);
        } while (!isUnique);

        var session = new TrainingSession
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            SessionCode = sessionCode,
            SessionTitle = request.SessionTitle,
            SessionType = request.SessionType,
            SessionDate = request.SessionDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            FacilityId = request.FacilityId,
            CoachId = request.CoachId,
            Status = SessionStatus.Scheduled,
            CreatedAt = DateTime.UtcNow
        };

        await sessionRepository.AddAsync(session, cancellationToken);

        _logger.LogInformation("Training session {SessionCode} created successfully", sessionCode);

        var createdSession = await sessionRepository.GetByIdWithDetailsAsync(session.Id, cancellationToken);
        return Result<TrainingSessionDto>.Success(MapToDto(createdSession!));
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
