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

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<RankingDto>.Failure("Athlete not found.");
        }

        if (athlete.Ranking is null)
        {
            _logger.LogWarning("Ranking not found for athlete: {AthleteId}", request.AthleteId);
            return Result<RankingDto>.Failure("Ranking not found for this athlete.");
        }

        var dto = new RankingDto
        {
            Id = athlete.Ranking.Id,
            CurrentRank = athlete.Ranking.CurrentRank,
            StateRank = athlete.Ranking.StateRank,
            NationalRank = athlete.Ranking.NationalRank,
            InternationalRank = athlete.Ranking.InternationalRank,
            RankingAuthority = athlete.Ranking.RankingAuthority,
            CreatedAt = athlete.Ranking.CreatedAt,
            UpdatedAt = athlete.Ranking.UpdatedAt
        };

        return Result<RankingDto>.Success(dto);
    }
}
