using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentResultsQuery
{
    public class GetAssessmentResultsQueryHandler : IRequestHandler<GetAssessmentResultsQuery, Result<IReadOnlyList<AssessmentResultDto>>>
    {
        private readonly IAssessmentRepository _repository;
        private readonly ILogger<GetAssessmentResultsQueryHandler> _logger;

        public GetAssessmentResultsQueryHandler(
            IAssessmentRepository repository,
            ILogger<GetAssessmentResultsQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<IReadOnlyList<AssessmentResultDto>>> Handle(GetAssessmentResultsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting results for assessment: {AssessmentId}", request.AssessmentId);

            var results = await _repository.GetResultsByAssessmentIdAsync(request.AssessmentId, cancellationToken);

            var dtos = results.Select(r => new AssessmentResultDto
            {
                Id = r.Id,
                AssessmentId = r.AssessmentId,
                AssessmentName = r.Assessment?.AssessmentName ?? string.Empty,
                AthleteId = r.AthleteId,
                AthleteName = r.Athlete?.User?.FullName ?? string.Empty,
                Score = r.Score,
                IsPassed = r.IsPassed,
                Remarks = r.Remarks,
                AssessedAt = r.AssessedAt
            }).ToList();

            return Result<IReadOnlyList<AssessmentResultDto>>.Success(dtos);
        }
    }
}
