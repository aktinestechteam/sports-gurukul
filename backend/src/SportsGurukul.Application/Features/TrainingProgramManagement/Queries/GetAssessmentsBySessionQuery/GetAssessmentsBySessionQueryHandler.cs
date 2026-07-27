using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetAssessmentsBySessionQuery;

public class GetAssessmentsBySessionQueryHandler : IRequestHandler<GetAssessmentsBySessionQuery, Result<IReadOnlyList<AssessmentDto>>>
{
    private readonly IAssessmentRepository _repository;
    private readonly ILogger<GetAssessmentsBySessionQueryHandler> _logger;

    public GetAssessmentsBySessionQueryHandler(
        IAssessmentRepository repository,
        ILogger<GetAssessmentsBySessionQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AssessmentDto>>> Handle(GetAssessmentsBySessionQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting assessments for session ID: {SessionId}", request.SessionId);

        var assessments = await _repository.GetBySessionIdAsync(request.SessionId, cancellationToken);

        var dtos = assessments.Select(AssessmentDto.MapToDto).ToList();

        return Result<IReadOnlyList<AssessmentDto>>.Success(dtos);
    }
}
