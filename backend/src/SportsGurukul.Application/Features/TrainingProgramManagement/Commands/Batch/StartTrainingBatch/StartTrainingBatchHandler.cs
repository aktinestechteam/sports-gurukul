using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.StartTrainingBatch;

public class StartTrainingBatchCommandHandler(
    ILogger<StartTrainingBatchCommandHandler> logger,
    ITrainingBatchRepository batchRepository
) : IRequestHandler<StartTrainingBatchCommand, Result<TrainingBatchDto>>
{
    private readonly ILogger<StartTrainingBatchCommandHandler> _logger = logger;

    public async Task<Result<TrainingBatchDto>> Handle(StartTrainingBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting training batch {BatchId}", request.Id);

        var batch = await batchRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Training batch with ID {BatchId} not found", request.Id);
            return Result<TrainingBatchDto>.Failure($"Training batch with ID {request.Id} not found");
        }

        if (batch.Status != BatchStatus.Waitlisted && batch.Status != BatchStatus.Inactive)
        {
            _logger.LogWarning("Training batch {BatchCode} cannot be started because status is {Status}", batch.BatchCode, batch.Status);
            return Result<TrainingBatchDto>.Failure("Training batch can only be started when status is Waitlisted or Inactive");
        }

        batch.Status = BatchStatus.Active;
        batch.UpdatedAt = DateTime.UtcNow;

        batchRepository.Update(batch);

        _logger.LogInformation("Training batch {BatchCode} started successfully", batch.BatchCode);

        var startedBatch = await batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
        return Result<TrainingBatchDto>.Success(MapToDto(startedBatch!));
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
