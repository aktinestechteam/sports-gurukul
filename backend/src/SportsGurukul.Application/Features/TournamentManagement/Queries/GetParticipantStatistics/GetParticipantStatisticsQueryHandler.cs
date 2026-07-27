using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetParticipantStatistics;

public class GetParticipantStatisticsQueryHandler : IRequestHandler<GetParticipantStatisticsQuery, Result<ParticipantStatisticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IRankingRepository _rankingRepository;
    private readonly ILogger<GetParticipantStatisticsQueryHandler> _logger;

    public GetParticipantStatisticsQueryHandler(
        IApplicationDbContext context,
        IRankingRepository rankingRepository,
        ILogger<GetParticipantStatisticsQueryHandler> logger)
    {
        _context = context;
        _rankingRepository = rankingRepository;
        _logger = logger;
    }

    public async Task<Result<ParticipantStatisticsDto>> Handle(GetParticipantStatisticsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting statistics for participant: {ParticipantId} in tournament: {TournamentId}", request.ParticipantId, request.TournamentId);

        var matches = await _context.TournamentMatches
            .AsNoTracking()
            .Where(m => m.TournamentId == request.TournamentId &&
                   (m.HomeParticipantId == request.ParticipantId || m.AwayParticipantId == request.ParticipantId) &&
                   m.Status == MatchStatus.Completed &&
                   !m.IsDeleted)
            .ToListAsync(cancellationToken);

        var ranking = await _rankingRepository.GetByParticipantAsync(request.TournamentId, request.ParticipantId, cancellationToken);

        var dto = new ParticipantStatisticsDto
        {
            ParticipantId = request.ParticipantId,
            ParticipantName = ranking?.Participant?.ParticipantName ?? "Unknown",
            MatchesPlayed = matches.Count,
            Wins = matches.Count(m => m.WinnerId == request.ParticipantId),
            Losses = matches.Count(m => m.WinnerId.HasValue && m.WinnerId != request.ParticipantId),
            Draws = matches.Count(m => !m.WinnerId.HasValue),
            Points = ranking?.Points ?? 0,
            CurrentRank = ranking?.Rank ?? 0
        };

        return Result<ParticipantStatisticsDto>.Success(dto);
    }
}
