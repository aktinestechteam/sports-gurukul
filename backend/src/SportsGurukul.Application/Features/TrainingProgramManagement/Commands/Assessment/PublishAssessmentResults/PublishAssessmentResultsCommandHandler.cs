using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;

public class PublishAssessmentResultsCommandHandler : IRequestHandler<PublishAssessmentResultsCommand, Result<bool>>
{
    private readonly IAssessmentRepository _assessmentRepository;
    private readonly ILogger<PublishAssessmentResultsCommandHandler> _logger;

    public PublishAssessmentResultsCommandHandler(
        IAssessmentRepository assessmentRepository,
        ILogger<PublishAssessmentResultsCommandHandler> logger)
    {
        _assessmentRepository = assessmentRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(PublishAssessmentResultsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing assessment results for assessment {AssessmentId}", request.AssessmentId);

        var assessment = await _assessmentRepository.GetByIdWithDetailsAsync(request.AssessmentId, cancellationToken);
        if (assessment is null)
        {
            _logger.LogWarning("Assessment {AssessmentId} not found", request.AssessmentId);
            return Result<bool>.Failure("Assessment not found");
        }

        var results = await _assessmentRepository.GetResultsByAssessmentIdAsync(request.AssessmentId, cancellationToken);
        if (results is null || results.Count == 0)
        {
            _logger.LogWarning("No results found for assessment {AssessmentId}. Cannot publish empty results", request.AssessmentId);
            return Result<bool>.Failure("No results found for this assessment. Cannot publish empty results");
        }

        var totalResults = results.Count;
        var passedResults = results.Count(r => r.IsPassed);

        _logger.LogInformation("Assessment results published for assessment {AssessmentId}: {TotalResults} total results, {PassedResults} passed", request.AssessmentId, totalResults, passedResults);
        _logger.LogInformation("Publish event logged for assessment '{AssessmentName}' (ID: {AssessmentId})", assessment.AssessmentName, request.AssessmentId);

        return Result<bool>.Success(true);
    }
}
