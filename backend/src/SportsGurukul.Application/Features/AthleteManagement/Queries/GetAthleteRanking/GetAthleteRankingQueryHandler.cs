using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteRanking;

public class GetAthleteRankingQueryHandler : IRequestHandler<GetAthleteRankingQuery, Result<RankingDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly ILogger<GetAthleteRankingQueryHandler> _logger;

    public GetAthleteRankingQueryHandler(
        IAthleteRepository athleteRepository,
        ILogger<GetAthleteRankingQueryHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _logger = logger;
    }

    public async Task<Result<RankingDto>> Handle(GetAthleteRankingQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching ranking for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<RankingDto>.Failure("Athlete not found.");
        }

        var ranking = await _athleteRepository.GetAthleteRankingAsync(request.AthleteId, cancellationToken);
        if (ranking is null)
        {
            _logger.LogWarning("Ranking not found for athlete: {AthleteId}", request.AthleteId);
            return Result<RankingDto>.Failure("Ranking not found for this athlete.");
        }

        var dto = new RankingDto
        {
            Id = ranking.Id,
            CurrentRank = ranking.CurrentRank,
            StateRank = ranking.StateRank,
            NationalRank = ranking.NationalRank,
            InternationalRank = ranking.InternationalRank,
            RankingAuthority = ranking.RankingAuthority,
            CreatedAt = ranking.CreatedAt,
            UpdatedAt = ranking.UpdatedAt
        };

        return Result<RankingDto>.Success(dto);
    }
}
