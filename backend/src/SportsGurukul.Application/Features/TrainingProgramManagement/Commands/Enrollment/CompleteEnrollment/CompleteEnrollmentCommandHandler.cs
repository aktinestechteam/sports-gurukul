using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CompleteEnrollment;

public class CompleteEnrollmentCommandHandler : IRequestHandler<CompleteEnrollmentCommand, Result<DTOs.EnrollmentDto>>
{
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ITrainingProgressRepository _progressRepository;
    private readonly ILogger<CompleteEnrollmentCommandHandler> _logger;

    public CompleteEnrollmentCommandHandler(
        ITrainingBatchRepository batchRepository,
        ITrainingProgressRepository progressRepository,
        ILogger<CompleteEnrollmentCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _progressRepository = progressRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.EnrollmentDto>> Handle(CompleteEnrollmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Completing enrollment {EnrollmentId} in batch {BatchId}", request.EnrollmentId, request.BatchId);

        var batch = await _batchRepository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);
        if (batch is null)
        {
            _logger.LogWarning("Batch {BatchId} not found", request.BatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Batch not found");
        }

        var enrollment = batch.Enrollments?
            .FirstOrDefault(e => e.Id == request.EnrollmentId);
        if (enrollment is null)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} not found in batch {BatchId}", request.EnrollmentId, request.BatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Enrollment not found");
        }

        if (enrollment.Status != EnrollmentStatus.Active)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} cannot be completed. Current status: {Status}", request.EnrollmentId, enrollment.Status);
            return Result<DTOs.EnrollmentDto>.Failure("Only active enrollments can be completed");
        }

        enrollment.Status = EnrollmentStatus.Completed;
        enrollment.UpdatedAt = DateTime.UtcNow;

        var existingProgress = await _progressRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
        if (existingProgress is not null)
        {
            existingProgress.CompletedPercentage = 100;
            existingProgress.CurrentLevel = "Completed";
            existingProgress.UpdatedAt = DateTime.UtcNow;
            _progressRepository.Update(existingProgress);
        }
        else
        {
            var progress = new TrainingProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollment.Id,
                CurrentLevel = "Completed",
                CompletedPercentage = 100,
                CreatedAt = DateTime.UtcNow
            };
            await _progressRepository.AddAsync(progress, cancellationToken);
            enrollment.Progress = progress;
        }

        _batchRepository.Update(batch);

        var athlete = enrollment.Athlete;
        var progressDto = enrollment.Progress is not null ? new DTOs.TrainingProgressDto
        {
            Id = enrollment.Progress.Id,
            EnrollmentId = enrollment.Progress.EnrollmentId,
            CurrentLevel = enrollment.Progress.CurrentLevel,
            CompletedPercentage = enrollment.Progress.CompletedPercentage,
            OverallRating = enrollment.Progress.OverallRating,
            CreatedAt = enrollment.Progress.CreatedAt,
            UpdatedAt = enrollment.Progress.UpdatedAt
        } : null;

        var dto = new DTOs.EnrollmentDto
        {
            Id = enrollment.Id,
            BatchId = enrollment.BatchId,
            BatchCode = batch.BatchCode,
            ProgramName = batch.Program?.ProgramName ?? string.Empty,
            AthleteId = enrollment.AthleteId,
            AthleteName = athlete?.User?.FullName ?? string.Empty,
            AthleteCode = athlete?.AthleteCode ?? string.Empty,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status.ToString(),
            Progress = progressDto,
            CreatedAt = enrollment.CreatedAt,
            UpdatedAt = enrollment.UpdatedAt
        };

        _logger.LogInformation("Enrollment {EnrollmentId} successfully completed", request.EnrollmentId);
        return Result<DTOs.EnrollmentDto>.Success(dto);
    }
}
