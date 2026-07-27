using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CreateTrainingBatch;

public class CreateTrainingBatchCommandHandler(
    ILogger<CreateTrainingBatchCommandHandler> logger,
    ITrainingProgramRepository programRepository,
    ITrainingBatchRepository batchRepository,
    ICoachRepository coachRepository,
    IAcademyBranchRepository branchRepository
) : IRequestHandler<CreateTrainingBatchCommand, Result<TrainingBatchDto>>
{
    private readonly ILogger<CreateTrainingBatchCommandHandler> _logger = logger;

    public async Task<Result<TrainingBatchDto>> Handle(CreateTrainingBatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating training batch for program {ProgramId}", request.ProgramId);

        var program = await programRepository.GetByIdAsync(request.ProgramId, cancellationToken);
        if (program == null)
        {
            _logger.LogWarning("Training program with ID {ProgramId} not found", request.ProgramId);
            return Result<TrainingBatchDto>.Failure($"Training program with ID {request.ProgramId} not found");
        }

        var coach = await coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach == null)
        {
            _logger.LogWarning("Coach with ID {CoachId} not found", request.CoachId);
            return Result<TrainingBatchDto>.Failure($"Coach with ID {request.CoachId} not found");
        }

        var branch = await branchRepository.GetByIdAsync(request.BranchId, cancellationToken);
        if (branch == null)
        {
            _logger.LogWarning("Academy branch with ID {BranchId} not found", request.BranchId);
            return Result<TrainingBatchDto>.Failure($"Academy branch with ID {request.BranchId} not found");
        }

        if (request.EndDate.HasValue && request.StartDate >= request.EndDate.Value)
        {
            _logger.LogWarning("StartDate must be before EndDate");
            return Result<TrainingBatchDto>.Failure("Start date must be before end date");
        }

        string batchCode;
        bool isUnique;
        do
        {
            batchCode = $"BAT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
            isUnique = await batchRepository.IsBatchCodeUniqueAsync(batchCode, cancellationToken);
        } while (!isUnique);

        var batch = new TrainingBatch
        {
            Id = Guid.NewGuid(),
            ProgramId = request.ProgramId,
            CoachId = request.CoachId,
            BranchId = request.BranchId,
            BatchCode = batchCode,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaximumSeats = request.MaximumSeats,
            Status = BatchStatus.Waitlisted,
            CreatedAt = DateTime.UtcNow
        };

        await batchRepository.AddAsync(batch, cancellationToken);

        _logger.LogInformation("Training batch {BatchCode} created successfully", batchCode);

        var createdBatch = await batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
        return Result<TrainingBatchDto>.Success(MapToDto(createdBatch!));
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
