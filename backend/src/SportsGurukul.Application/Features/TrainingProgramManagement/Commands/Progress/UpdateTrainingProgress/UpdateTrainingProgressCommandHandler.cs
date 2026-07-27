using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;

public class UpdateTrainingProgressCommandHandler : IRequestHandler<UpdateTrainingProgressCommand, Result<DTOs.TrainingProgressDto>>
{
    private readonly ITrainingProgressRepository _progressRepository;
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ILogger<UpdateTrainingProgressCommandHandler> _logger;

    public UpdateTrainingProgressCommandHandler(
        ITrainingProgressRepository progressRepository,
        ITrainingBatchRepository batchRepository,
        ILogger<UpdateTrainingProgressCommandHandler> logger)
    {
        _progressRepository = progressRepository;
        _batchRepository = batchRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.TrainingProgressDto>> Handle(UpdateTrainingProgressCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating training progress for enrollment {EnrollmentId}", request.EnrollmentId);

        var enrollmentFound = false;
        var batches = await _batchRepository.GetAllAsync(cancellationToken);
        foreach (var batch in batches)
        {
            var details = await _batchRepository.GetByIdWithDetailsAsync(batch.Id, cancellationToken);
            if (details?.Enrollments?.Any(e => e.Id == request.EnrollmentId) == true)
            {
                enrollmentFound = true;
                var enrollment = details.Enrollments.First(e => e.Id == request.EnrollmentId);

                if (enrollment.Status != EnrollmentStatus.Active && enrollment.Status != EnrollmentStatus.Completed)
                {
                    _logger.LogWarning("Enrollment {EnrollmentId} is not active or completed. Current status: {Status}", request.EnrollmentId, enrollment.Status);
                    return Result<DTOs.TrainingProgressDto>.Failure("Enrollment is not in a valid state for progress update");
                }
                break;
            }
        }

        if (!enrollmentFound)
        {
            _logger.LogWarning("Enrollment {EnrollmentId} not found", request.EnrollmentId);
            return Result<DTOs.TrainingProgressDto>.Failure("Enrollment not found");
        }

        if (request.CompletedPercentage < 0 || request.CompletedPercentage > 100)
        {
            _logger.LogWarning("Completed percentage must be between 0 and 100. Received: {Percentage}", request.CompletedPercentage);
            return Result<DTOs.TrainingProgressDto>.Failure("Completed percentage must be between 0 and 100");
        }

        var existingProgress = await _progressRepository.GetByEnrollmentIdAsync(request.EnrollmentId, cancellationToken);
        TrainingProgress progress;

        if (existingProgress is not null)
        {
            existingProgress.CurrentLevel = request.CurrentLevel;
            existingProgress.CompletedPercentage = request.CompletedPercentage;
            existingProgress.OverallRating = request.OverallRating;
            existingProgress.UpdatedAt = DateTime.UtcNow;
            _progressRepository.Update(existingProgress);
            progress = existingProgress;
            _logger.LogInformation("Existing progress {ProgressId} updated for enrollment {EnrollmentId}", progress.Id, request.EnrollmentId);
        }
        else
        {
            progress = new TrainingProgress
            {
                Id = Guid.NewGuid(),
                EnrollmentId = request.EnrollmentId,
                CurrentLevel = request.CurrentLevel,
                CompletedPercentage = request.CompletedPercentage,
                OverallRating = request.OverallRating,
                CreatedAt = DateTime.UtcNow
            };
            await _progressRepository.AddAsync(progress, cancellationToken);
            _logger.LogInformation("New progress {ProgressId} created for enrollment {EnrollmentId}", progress.Id, request.EnrollmentId);
        }

        var dto = new DTOs.TrainingProgressDto
        {
            Id = progress.Id,
            EnrollmentId = progress.EnrollmentId,
            CurrentLevel = progress.CurrentLevel,
            CompletedPercentage = progress.CompletedPercentage,
            OverallRating = progress.OverallRating,
            CreatedAt = progress.CreatedAt,
            UpdatedAt = progress.UpdatedAt
        };

        _logger.LogInformation("Training progress for enrollment {EnrollmentId} successfully updated", request.EnrollmentId);
        return Result<DTOs.TrainingProgressDto>.Success(dto);
    }
}
