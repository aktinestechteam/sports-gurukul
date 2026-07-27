using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;

public class CancelEnrollmentCommandHandler : IRequestHandler<CancelEnrollmentCommand, Result<DTOs.EnrollmentDto>>
{
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly ILogger<CancelEnrollmentCommandHandler> _logger;

    public CancelEnrollmentCommandHandler(
        ITrainingBatchRepository batchRepository,
        ILogger<CancelEnrollmentCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.EnrollmentDto>> Handle(CancelEnrollmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling enrollment {EnrollmentId} in batch {BatchId}", request.EnrollmentId, request.BatchId);

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
            _logger.LogWarning("Enrollment {EnrollmentId} cannot be cancelled. Current status: {Status}", request.EnrollmentId, enrollment.Status);
            return Result<DTOs.EnrollmentDto>.Failure("Only active enrollments can be cancelled");
        }

        enrollment.Status = EnrollmentStatus.Withdrawn;
        enrollment.UpdatedAt = DateTime.UtcNow;
        _batchRepository.Update(batch);

        var athlete = enrollment.Athlete;
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
            CreatedAt = enrollment.CreatedAt,
            UpdatedAt = enrollment.UpdatedAt
        };

        _logger.LogInformation("Enrollment {EnrollmentId} successfully cancelled", request.EnrollmentId);
        return Result<DTOs.EnrollmentDto>.Success(dto);
    }
}
