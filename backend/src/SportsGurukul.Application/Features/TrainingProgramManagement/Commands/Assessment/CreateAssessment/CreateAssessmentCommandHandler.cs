using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;

public class CreateAssessmentCommandHandler : IRequestHandler<CreateAssessmentCommand, Result<DTOs.AssessmentDto>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly ILogger<CreateAssessmentCommandHandler> _logger;

    public CreateAssessmentCommandHandler(
        ISessionRepository sessionRepository,
        IAssessmentRepository assessmentRepository,
        ILogger<CreateAssessmentCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _assessmentRepository = assessmentRepository;
        _logger = logger;
    }

    public async Task<Result<DTOs.AssessmentDto>> Handle(CreateAssessmentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating assessment '{AssessmentName}' for session {SessionId}", request.AssessmentName, request.SessionId);

        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", request.SessionId);
            return Result<DTOs.AssessmentDto>.Failure("Session not found");
        }

        if (request.MaximumScore <= 0)
        {
            _logger.LogWarning("Maximum score must be greater than zero. Received: {MaximumScore}", request.MaximumScore);
            return Result<DTOs.AssessmentDto>.Failure("Maximum score must be greater than zero");
        }

        if (request.PassingScore <= 0 || request.PassingScore > request.MaximumScore)
        {
            _logger.LogWarning("Passing score must be between 0 and maximum score {MaximumScore}. Received: {PassingScore}", request.MaximumScore, request.PassingScore);
            return Result<DTOs.AssessmentDto>.Failure("Passing score must be between 0 and the maximum score");
        }

        var assessment = new TrainingAssessment
        {
            Id = Guid.NewGuid(),
            SessionId = request.SessionId,
            AssessmentType = Enum.Parse<AssessmentType>(request.AssessmentType),
            AssessmentName = request.AssessmentName,
            MaximumScore = request.MaximumScore,
            PassingScore = request.PassingScore,
            CreatedAt = DateTime.UtcNow
        };

        await _assessmentRepository.AddAsync(assessment, cancellationToken);

        var dto = new DTOs.AssessmentDto
        {
            Id = assessment.Id,
            SessionId = assessment.SessionId,
            SessionCode = session.SessionCode,
            AssessmentType = assessment.AssessmentType.ToString(),
            AssessmentName = assessment.AssessmentName,
            MaximumScore = assessment.MaximumScore,
            PassingScore = assessment.PassingScore,
            Results = new System.Collections.Generic.List<DTOs.AssessmentResultDto>()
        };

        _logger.LogInformation("Assessment {AssessmentId} successfully created for session {SessionId}", assessment.Id, request.SessionId);
        return Result<DTOs.AssessmentDto>.Success(dto);
    }
}
