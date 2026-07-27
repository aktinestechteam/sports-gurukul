using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;

public class CancelTrainingBatchCommandHandler(
    ILogger<CancelTrainingBatchCommandHandler> logger,
    ITrainingBatchRepository batchRepository
) : IRequestHandler<CancelTrainingBatchCommand, Result<TrainingBatchDto>>
{
    private readonly ILogger<CancelTrainingBatchCommandHandler> _logger = logger;

    public async Task<Result<TrainingBatchDto>> Handle(CancelTrainingBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling training batch {BatchId}", request.Id);

        var batch = await batchRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Training batch with ID {BatchId} not found", request.Id);
            return Result<TrainingBatchDto>.Failure($"Training batch with ID {request.Id} not found");
        }

        if (batch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Training batch {BatchCode} cannot be cancelled because status is {Status}", batch.BatchCode, batch.Status);
            return Result<TrainingBatchDto>.Failure("Training batch can only be cancelled when status is Active");
        }

        batch.Status = BatchStatus.Inactive;
        batch.UpdatedAt = DateTime.UtcNow;

        batchRepository.Update(batch);

        _logger.LogInformation("Training batch {BatchCode} cancelled successfully", batch.BatchCode);

        var cancelledBatch = await batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
        return Result<TrainingBatchDto>.Success(MapToDto(cancelledBatch!));
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
