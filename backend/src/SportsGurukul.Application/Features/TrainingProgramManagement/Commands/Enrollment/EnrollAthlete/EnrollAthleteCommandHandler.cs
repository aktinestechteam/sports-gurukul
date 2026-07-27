using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;

public class EnrollAthleteCommandHandler : IRequestHandler<EnrollAthleteCommand, Result<DTOs.EnrollmentDto>>
{
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<EnrollAthleteCommandHandler> _logger;

    public EnrollAthleteCommandHandler(
        ITrainingBatchRepository batchRepository,
        IAthleteRepository athleteRepository,
        ILogger<EnrollAthleteCommandHandler> logger)
    {
        _batchRepository = batchRepository;
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.EnrollmentDto>> Handle(EnrollAthleteCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enrolling athlete {AthleteId} in batch {BatchId}", request.AthleteId, request.BatchId);

        var batch = await _batchRepository.GetByIdWithDetailsAsync(request.BatchId, cancellationToken);
        if (batch is null)
        {
            _logger.LogWarning("Batch {BatchId} not found", request.BatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Batch not found");
        }

        if (batch.Status != BatchStatus.Active)
        {
            _logger.LogWarning("Batch {BatchId} is not active. Current status: {Status}", request.BatchId, batch.Status);
            return Result<DTOs.EnrollmentDto>.Failure("Batch is not active");
        }

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete {AthleteId} not found", request.AthleteId);
            return Result<DTOs.EnrollmentDto>.Failure("Athlete not found");
        }

        var existingEnrollment = batch.Enrollments?
            .FirstOrDefault(e => e.AthleteId == request.AthleteId && e.Status == EnrollmentStatus.Active);
        if (existingEnrollment is not null)
        {
            _logger.LogWarning("Athlete {AthleteId} is already actively enrolled in batch {BatchId}", request.AthleteId, request.BatchId);
            return Result<DTOs.EnrollmentDto>.Failure("Athlete is already enrolled in this batch");
        }

        var activeEnrollments = batch.Enrollments?.Count(e => e.Status == EnrollmentStatus.Active) ?? 0;
        if (activeEnrollments >= batch.MaximumSeats)
        {
            _logger.LogWarning("Batch {BatchId} has reached maximum capacity of {MaxSeats}", request.BatchId, batch.MaximumSeats);
            return Result<DTOs.EnrollmentDto>.Failure("Batch has reached maximum capacity");
        }

        var enrollment = new TrainingEnrollment
        {
            Id = Guid.NewGuid(),
            BatchId = request.BatchId,
            AthleteId = request.AthleteId,
            EnrollmentDate = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        batch.Enrollments ??= new List<TrainingEnrollment>();
        batch.Enrollments.Add(enrollment);
        _batchRepository.Update(batch);

        var dto = new DTOs.EnrollmentDto
        {
            Id = enrollment.Id,
            BatchId = enrollment.BatchId,
            BatchCode = batch.BatchCode,
            ProgramName = batch.Program?.ProgramName ?? string.Empty,
            AthleteId = enrollment.AthleteId,
            AthleteName = athlete.User?.FullName ?? string.Empty,
            AthleteCode = athlete.AthleteCode,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status.ToString(),
            CreatedAt = enrollment.CreatedAt
        };

        _logger.LogInformation("Athlete {AthleteId} successfully enrolled in batch {BatchId} with enrollment {EnrollmentId}", request.AthleteId, request.BatchId, enrollment.Id);
        return Result<DTOs.EnrollmentDto>.Success(dto);
    }
}
