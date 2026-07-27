using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.UpdateTrainingSession;

public class UpdateTrainingSessionCommandHandler(
    ILogger<UpdateTrainingSessionCommandHandler> logger,
    ISessionRepository sessionRepository
) : IRequestHandler<UpdateTrainingSessionCommand, Result<TrainingSessionDto>>
{
    private readonly ILogger<UpdateTrainingSessionCommandHandler> _logger = logger;

    public async Task<Result<TrainingSessionDto>> Handle(UpdateTrainingSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training session {SessionId}", request.Id);

        var session = await sessionRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (session == null)
        {
            _logger.LogWarning("Training session with ID {SessionId} not found", request.Id);
            return Result<TrainingSessionDto>.Failure($"Training session with ID {request.Id} not found");
        }

        if (session.Status != SessionStatus.Scheduled)
        {
            _logger.LogWarning("Training session {SessionCode} cannot be updated because status is {Status}", session.SessionCode, session.Status);
            return Result<TrainingSessionDto>.Failure("Training session can only be updated when status is Scheduled");
        }

        if (request.StartTime >= request.EndTime)
        {
            _logger.LogWarning("StartTime must be before EndTime");
            return Result<TrainingSessionDto>.Failure("Start time must be before end time");
        }

        session.SessionTitle = request.SessionTitle;
        session.SessionType = request.SessionType;
        session.SessionDate = request.SessionDate;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.UpdatedAt = DateTime.UtcNow;

        sessionRepository.Update(session);

        _logger.LogInformation("Training session {SessionCode} updated successfully", session.SessionCode);

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
