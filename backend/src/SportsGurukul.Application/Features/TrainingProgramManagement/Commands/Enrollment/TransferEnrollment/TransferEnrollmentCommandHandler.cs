using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;

public class TransferEnrollmentCommandHandler : IRequestHandler<TransferEnrollmentCommand, Result<DTOs.EnrollmentDto>>
{
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ILogger<TransferEnrollmentCommandHandler> _logger;

    public TransferEnrollmentCommandHandler(
        ITrainingBatchRepository batchRepository,
        ILogger<TransferEnrollmentCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.EnrollmentDto>> Handle(TransferEnrollmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transferring enrollment {EnrollmentId} from batch {SourceBatchId} to batch {TargetBatchId}", request.EnrollmentId, request.SourceBatchId, request.TargetBatchId);

        if (request.SourceBatchId == request.TargetBatchId)
        {
            _logger.LogWarning("Source and target batch are the same: {BatchId}", request.SourceBatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Source and target batch cannot be the same");
        }

        var sourceBatch = await _batchRepository.GetByIdWithDetailsAsync(request.SourceBatchId, cancellationToken);
        if (sourceBatch is null)
        {
            _logger.LogWarning("Source batch {SourceBatchId} not found", request.SourceBatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Source batch not found");
        }

        var enrollment = sourceBatch.Enrollments?
            .FirstOrDefault(e => e.Id == request.EnrollmentId && e.Status == EnrollmentStatus.Active);
        if (enrollment is null)
        {
            _logger.LogWarning("Active enrollment {EnrollmentId} not found in source batch {SourceBatchId}", request.EnrollmentId, request.SourceBatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Active enrollment not found in source batch");
        }

        var targetBatch = await _batchRepository.GetByIdWithDetailsAsync(request.TargetBatchId, cancellationToken);
        if (targetBatch is null)
        {
            _logger.LogWarning("Target batch {TargetBatchId} not found", request.TargetBatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Target batch not found");
        }

        if (targetBatch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Target batch {TargetBatchId} is not active. Current status: {Status}", request.TargetBatchId, targetBatch.Status);
            return Result<DTOs.EnrollmentDto>.Failure("Target batch is not active");
        }

        var targetHasAthlete = targetBatch.Enrollments?
            .Any(e => e.AthleteId == enrollment.AthleteId && e.Status == EnrollmentStatus.Active) ?? false;
        if (targetHasAthlete)
        {
            _logger.LogWarning("Athlete {AthleteId} is already actively enrolled in target batch {TargetBatchId}", enrollment.AthleteId, request.TargetBatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Athlete is already enrolled in the target batch");
        }

        var targetActiveCount = targetBatch.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
        if (targetActiveCount >= targetBatch.MaximumSeats)
        {
            _logger.LogWarning("Target batch {TargetBatchId} has reached maximum capacity of {MaxSeats}", request.TargetBatchId, targetBatch.MaximumSeats);
            return Result<DTOs.EnrollmentDto>.Failure("Target batch has reached maximum capacity");
        }

        sourceBatch.Enrollments?.Remove(enrollment);
        enrollment.BatchId = request.TargetBatchId;
        enrollment.UpdatedAt = DateTime.UtcNow;
        targetBatch.Enrollments ??= new List<Domain.Entities.TrainingEnrollment>();
        targetBatch.Enrollments.Add(enrollment);

        _batchRepository.Update(sourceBatch);
        _batchRepository.Update(targetBatch);

        var athlete = enrollment.Athlete;
        var dto = new DTOs.EnrollmentDto
        {
            Id = enrollment.Id,
            BatchId = enrollment.BatchId,
            BatchCode = targetBatch.BatchCode,
            ProgramName = targetBatch.Program?.ProgramName ?? string.Empty,
            AthleteId = enrollment.AthleteId,
            AthleteName = athlete?.User?.FullName ?? string.Empty,
            AthleteCode = athlete?.AthleteCode ?? string.Empty,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status.ToString(),
            CreatedAt = enrollment.CreatedAt,
            UpdatedAt = enrollment.UpdatedAt
        };

        _logger.LogInformation("Enrollment {EnrollmentId} successfully transferred from batch {SourceBatchId} to batch {TargetBatchId}", request.EnrollmentId, request.SourceBatchId, request.TargetBatchId);
        return Result<DTOs.EnrollmentDto>.Success(dto);
    }
}
