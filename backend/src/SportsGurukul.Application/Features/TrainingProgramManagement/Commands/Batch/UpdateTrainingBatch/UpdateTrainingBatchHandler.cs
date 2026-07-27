using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;

public class UpdateTrainingBatchCommandHandler(
    ILogger<UpdateTrainingBatchCommandHandler> logger,
    ITrainingBatchRepository batchRepository
) : IRequestHandler<UpdateTrainingBatchCommand, Result<TrainingBatchDto>>
{
    private readonly ILogger<UpdateTrainingBatchCommandHandler> _logger = logger;

    public async Task<Result<TrainingBatchDto>> Handle(UpdateTrainingBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training batch {BatchId}", request.Id);

        var batch = await batchRepository.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (batch == null)
        {
            _logger.LogWarning("Training batch with ID {BatchId} not found", request.Id);
            return Result<TrainingBatchDto>.Failure($"Training batch with ID {request.Id} not found");
        }

        if (batch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Training batch {BatchCode} cannot be updated because status is {Status}", batch.BatchCode, batch.Status);
            return Result<TrainingBatchDto>.Failure("Training batch can only be updated when status is Active");
        }

        if (request.EndDate.HasValue && request.StartDate >= request.EndDate.Value)
        {
            _logger.LogWarning("StartDate must be before EndDate");
            return Result<TrainingBatchDto>.Failure("Start date must be before end date");
        }

        batch.StartDate = request.StartDate;
        batch.EndDate = request.EndDate;
        batch.MaximumSeats = request.MaximumSeats;
        batch.UpdatedAt = DateTime.UtcNow;

        batchRepository.Update(batch);

        _logger.LogInformation("Training batch {BatchCode} updated successfully", batch.BatchCode);

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
