using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.AssignCoachToBatch;

public class AssignCoachToBatchCommandHandler(
    ILogger<AssignCoachToBatchCommandHandler> logger,
    ITrainingBatchRepository batchRepository,
    ICoachRepository coachRepository
) : IRequestHandler<AssignCoachToBatchCommand, Result<TrainingBatchDto>>
{
    private readonly ILogger<AssignCoachToBatchCommandHandler> _logger = logger;

    public async Task<Result<TrainingBatchDto>> Handle(AssignCoachToBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning coach {CoachId} to batch {BatchId}", request.CoachId, request.Id);

        var batch = await batchRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Training batch with ID {BatchId} not found", request.Id);
            return Result<TrainingBatchDto>.Failure($"Training batch with ID {request.Id} not found");
        }

        if (batch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Training batch {BatchCode} cannot reassign coach because status is {Status}", batch.BatchCode, batch.Status);
            return Result<TrainingBatchDto>.Failure("Coach can only be reassigned to Active training batches");
        }

        var newCoach = await coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (newCoach == null)
        {
            _logger.LogWarning("Coach with ID {CoachId} not found", request.CoachId);
            return Result<TrainingBatchDto>.Failure($"Coach with ID {request.CoachId} not found");
        }

        batch.CoachId = request.CoachId;
        batch.UpdatedAt = DateTime.UtcNow;

        batchRepository.Update(batch);

        _logger.LogInformation("Coach assigned to batch {BatchCode} successfully", batch.BatchCode);

        var updatedBatch = await batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
        return Result<TrainingBatchDto>.Success(MapToDto(updatedBatch!));
    }

    public static TrainingBatchDto MapToDto(TrainingBatch batch) => new()
    {
        Id = batch.Id,
        ProgramId = batch.ProgramId,
        ProgramName = batch.Program?.ProgramName ?? string.Empty,
        BatchCode = batch.BatchCode,
        CoachId = batch.CoachId,
        CoachName = batch.Coach?.User?.FullName ?? string.Empty,
        BranchId = batch.BranchId,
        BranchName = batch.Branch?.BranchName ?? string.Empty,
        StartDate = batch.StartDate,
        EndDate = batch.EndDate ?? DateTime.UtcNow,
        MaximumSeats = batch.MaximumSeats,
        EnrollmentCount = batch.Enrollments?.Count ?? 0,
        SessionsCount = batch.Sessions?.Count ?? 0,
        Status = batch.Status.ToString(),
        RowVersion = batch.RowVersion,
        CreatedAt = batch.CreatedAt,
        UpdatedAt = batch.UpdatedAt
    };
}
