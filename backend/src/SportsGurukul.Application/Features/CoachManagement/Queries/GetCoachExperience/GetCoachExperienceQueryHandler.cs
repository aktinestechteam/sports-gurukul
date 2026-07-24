using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachExperience;

public class GetCoachExperienceQueryHandler : IRequestHandler<GetCoachExperienceQuery, Result<IReadOnlyList<ExperienceDto>>>
{
    private readonly IRepository<CoachExperience> _coachExperienceRepository;
    private readonly ILogger<GetCoachExperienceQueryHandler> _logger;

    public GetCoachExperienceQueryHandler(
        IRepository<CoachExperience> coachExperienceRepository,
        ILogger<GetCoachExperienceQueryHandler> logger)
    {
        _coachExperienceRepository = coachExperienceRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ExperienceDto>>> Handle(GetCoachExperienceQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching experience records for coach: {CoachId}", request.CoachId);

        var experiences = await _coachExperienceRepository.FindAsync(
            x => x.CoachId == request.CoachId && !x.IsDeleted,
            cancellationToken);

        var dtos = experiences.Select(e => new ExperienceDto
        {
            Id = e.Id,
            Organization = e.Organization,
            Role = e.Role,
            Sport = e.Sport,
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            Description = e.Description,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt
        }).ToList();

        _logger.LogInformation("Retrieved {Count} experience records for coach: {CoachId}", dtos.Count, request.CoachId);

        return Result<IReadOnlyList<ExperienceDto>>.Success(dtos);
    }
}
