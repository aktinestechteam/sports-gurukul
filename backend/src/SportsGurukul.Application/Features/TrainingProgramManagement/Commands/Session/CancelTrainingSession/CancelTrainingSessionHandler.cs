using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Session.CancelTrainingSession;

public class CancelTrainingSessionCommandHandler(
    ILogger<CancelTrainingSessionCommandHandler> logger,
    ISessionRepository sessionRepository
) : IRequestHandler<CancelTrainingSessionCommand, Result<TrainingSessionDto>>
{
    private readonly ILogger<CancelTrainingSessionCommandHandler> _logger = logger;

    public async Task<Result<TrainingSessionDto>> Handle(CancelTrainingSessionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling training session {SessionId}", request.Id);

        var session = await sessionRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (session == null)
        {
            _logger.LogWarning("Training session with ID {SessionId} not found", request.Id);
            return Result<TrainingSessionDto>.Failure($"Training session with ID {request.Id} not found");
        }

        if (session.Status != SessionStatus.Scheduled)
        {
            _logger.LogWarning("Training session {SessionCode} cannot be cancelled because status is {Status}", session.SessionCode, session.Status);
            return Result<TrainingSessionDto>.Failure("Training session can only be cancelled when status is Scheduled");
        }

        session.Status = SessionStatus.Cancelled;
        session.UpdatedAt = DateTime.UtcNow;

        sessionRepository.Update(session);

        _logger.LogInformation("Training session {SessionCode} cancelled successfully", session.SessionCode);

        var cancelledSession = await sessionRepository.GetByIdWithDetailsAsync(session.Id, cancellationToken);
        return Result<TrainingSessionDto>.Success(MapToDto(cancelledSession!));
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
