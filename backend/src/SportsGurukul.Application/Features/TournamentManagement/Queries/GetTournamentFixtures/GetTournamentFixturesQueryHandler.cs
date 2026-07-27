using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentFixtures;

public class GetTournamentFixturesQueryHandler : IRequestHandler<GetTournamentFixturesQuery, Result<IReadOnlyList<FixtureDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTournamentFixturesQueryHandler> _logger;

    public GetTournamentFixturesQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTournamentFixturesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<FixtureDto>>> Handle(GetTournamentFixturesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting fixtures for tournament: {TournamentId}", request.TournamentId);

        var query = _context.TournamentFixtures
            .AsNoTracking()
            .Where(f => f.TournamentId == request.TournamentId && !f.IsDeleted);

        if (request.StageId.HasValue)
            query = query.Where(f => f.TournamentStageId == request.StageId.Value);

        var fixtures = await query
            .OrderBy(f => f.FixtureNumber)
            .ToListAsync(cancellationToken);

        var dtos = fixtures.Select(f => new FixtureDto
        {
            Id = f.Id,
            TournamentId = f.TournamentId,
            TournamentStageId = f.TournamentStageId,
            FixtureNumber = f.FixtureNumber,
            ScheduledDate = f.ScheduledDate,
            ScheduledTime = f.ScheduledTime,
            HomeTeamName = f.HomeTeamName,
            AwayTeamName = f.AwayTeamName,
            IsPublished = f.IsPublished,
            Notes = f.Notes,
            CreatedAt = f.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<FixtureDto>>.Success(dtos);
    }
}
