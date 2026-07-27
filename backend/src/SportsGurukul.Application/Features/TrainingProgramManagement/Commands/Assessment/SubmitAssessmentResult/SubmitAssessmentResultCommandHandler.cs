using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;

public class SubmitAssessmentResultCommandHandler : IRequestHandler<SubmitAssessmentResultCommand, Result<DTOs.AssessmentResultDto>>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly ITrainingBatchRepository _batchRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<SubmitAssessmentResultCommandHandler> _logger;

    public SubmitAssessmentResultCommandHandler(
        IAssessmentRepository assessmentRepository,
        ITrainingBatchRepository batchRepository,
        IAthleteRepository athleteRepository,
        ILogger<SubmitAssessmentResultCommandHandler> logger)
    {
        _assessmentRepository = assessmentRepository;
        _batchRepository = batchRepository;
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AssessmentResultDto>> Handle(SubmitAssessmentResultCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Submitting assessment result for athlete {AthleteId} on assessment {AssessmentId}", request.AthleteId, request.AssessmentId);

        var assessment = await _assessmentRepository.GetByIdWithDetailsAsync(request.AssessmentId, cancellationToken);
        if (assessment is null)
        {
            _logger.LogWarning("Assessment {AssessmentId} not found", request.AssessmentId);
            return Result<DTOs.AssessmentResultDto>.Failure("Assessment not found");
        }

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete {AthleteId} not found", request.AthleteId);
            return Result<DTOs.AssessmentResultDto>.Failure("Athlete not found");
        }

        var session = assessment.Session;
        if (session is null)
        {
            _logger.LogWarning("Session for assessment {AssessmentId} not found", request.AssessmentId);
            return Result<DTOs.AssessmentResultDto>.Failure("Associated session not found");
        }

        var batch = await _batchRepository.GetByIdWithDetailsAsync(session.BatchId, cancellationToken);
        if (batch is null)
        {
            _logger.LogWarning("Batch {BatchId} for session {SessionId} not found", session.BatchId, session.Id);
            return Result<DTOs.AssessmentResultDto>.Failure("Associated batch not found");
        }

        var isEnrolled = batch.Enrollments?
            .Any(e => e.AthleteId == request.AthleteId && e.Status == EnrollmentStatus.Active) ?? false;
        if (!isEnrolled)
        {
            _logger.LogWarning("Athlete {AthleteId} is not enrolled in batch {BatchId}", request.AthleteId, session.BatchId);
            return Result<DTOs.AssessmentResultDto>.Failure("Athlete is not enrolled in the associated batch");
        }

        var existingResult = assessment.Results?
            .FirstOrDefault(r => r.AthleteId == request.AthleteId);
        if (existingResult is not null)
        {
            _logger.LogWarning("Result already exists for athlete {AthleteId} on assessment {AssessmentId}", request.AthleteId, request.AssessmentId);
            return Result<DTOs.AssessmentResultDto>.Failure("A result already exists for this athlete on this assessment");
        }

        if (request.Score < 0 || request.Score > assessment.MaximumScore)
        {
            _logger.LogWarning("Score {Score} is out of range (0-{MaximumScore})", request.Score, assessment.MaximumScore);
            return Result<DTOs.AssessmentResultDto>.Failure($"Score must be between 0 and {assessment.MaximumScore}");
        }

        var result = new AssessmentResult
        {
            Id = Guid.NewGuid(),
            AssessmentId = request.AssessmentId,
            AthleteId = request.AthleteId,
            Score = request.Score,
            IsPassed = request.Score >= assessment.PassingScore,
            Remarks = request.Remarks,
            AssessedAt = DateTime.UtcNow
        };

        assessment.Results.Add(result);
        _assessmentRepository.Update(assessment);

        var dto = new DTOs.AssessmentResultDto
        {
            Id = result.Id,
            AssessmentId = result.AssessmentId,
            AssessmentName = assessment.AssessmentName,
            AthleteId = result.AthleteId,
            AthleteName = athlete.User?.FullName ?? string.Empty,
            Score = result.Score,
            IsPassed = result.IsPassed,
            Remarks = result.Remarks,
            AssessedAt = result.AssessedAt
        };

        _logger.LogInformation("Assessment result {ResultId} submitted for athlete {AthleteId} with score {Score} (Passed: {IsPassed})", result.Id, request.AthleteId, result.Score, result.IsPassed);
        return Result<DTOs.AssessmentResultDto>.Success(dto);
    }
}
