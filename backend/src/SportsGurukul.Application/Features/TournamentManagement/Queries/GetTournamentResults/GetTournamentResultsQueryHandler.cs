using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentResults;

public class GetTournamentResultsQueryHandler : IRequestHandler<GetTournamentResultsQuery, Result<IReadOnlyList<ResultDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetTournamentResultsQueryHandler> _logger;

    public GetTournamentResultsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetTournamentResultsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<ResultDto>>> Handle(GetTournamentResultsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting results for tournament: {TournamentId}", request.TournamentId);

        var results = await _context.TournamentResults
            .AsNoTracking()
            .Where(r => r.TournamentId == request.TournamentId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

        var dtos = results.Select(r => new ResultDto
        {
            Id = r.Id,
            TournamentId = r.TournamentId,
            MatchId = r.MatchId,
            WinnerId = r.WinnerId,
            WinnerName = r.WinnerName,
            HomeScore = r.HomeScore,
            AwayScore = r.AwayScore,
            ResultDetails = r.ResultDetails,
            IsVerified = r.IsVerified,
            Notes = r.Notes,
            CreatedAt = r.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<ResultDto>>.Success(dtos);
    }
}
