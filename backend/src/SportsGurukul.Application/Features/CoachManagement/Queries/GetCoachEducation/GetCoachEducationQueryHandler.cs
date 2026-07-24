using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachEducation;

public class GetCoachEducationQueryHandler : IRequestHandler<GetCoachEducationQuery, Result<IReadOnlyList<EducationDto>>>
{
    private readonly IRepository<CoachEducation> _coachEducationRepository;
    private readonly ILogger<GetCoachEducationQueryHandler> _logger;

    public GetCoachEducationQueryHandler(
        IRepository<CoachEducation> coachEducationRepository,
        ILogger<GetCoachEducationQueryHandler> logger)
    {
        _coachEducationRepository = coachEducationRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EducationDto>>> Handle(GetCoachEducationQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching education records for coach: {CoachId}", request.CoachId);

        var education = await _coachEducationRepository.FindAsync(
            x => x.CoachId == request.CoachId && !x.IsDeleted,
            cancellationToken);

        var dtos = education.Select(e => new EducationDto
        {
            Id = e.Id,
            Degree = e.Degree,
            Institution = e.Institution,
            FieldOfStudy = e.FieldOfStudy,
            YearCompleted = e.YearCompleted,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} education records for coach: {CoachId}", dtos.Count, request.CoachId);

        return Result<IReadOnlyList<EducationDto>>.Success(dtos);
    }
}
