using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;

public class UpdateRankingCommandHandler : IRequestHandler<UpdateRankingCommand, Result<RankingDto>>
{
    private readonly IAthleteRepository _athleteRepository;
    private readonly IRepository<Ranking> _rankingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateRankingCommandHandler> _logger;

    public UpdateRankingCommandHandler(
        IAthleteRepository athleteRepository,
        IRepository<Ranking> rankingRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateRankingCommandHandler> logger)
    {
        _athleteRepository = athleteRepository;
        _rankingRepository = rankingRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<RankingDto>> Handle(UpdateRankingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating ranking for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdWithDetailsAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
        {
            _logger.LogWarning("Athlete not found: {AthleteId}", request.AthleteId);
            return Result<RankingDto>.Failure("Athlete not found.");
        }

        var ranking = athlete.Ranking;
        if (ranking is null)
        {
            ranking = new Ranking
            {
                Id = Guid.NewGuid(),
                AthleteId = request.AthleteId
            };
            await _rankingRepository.AddAsync(ranking, cancellationToken);
        }

        ranking.CurrentRank = request.CurrentRank;
        ranking.StateRank = request.StateRank;
        ranking.NationalRank = request.NationalRank;
        ranking.InternationalRank = request.InternationalRank;
        ranking.RankingAuthority = request.RankingAuthority;
        ranking.UpdatedAt = DateTime.UtcNow;

        _rankingRepository.Update(ranking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Ranking updated for athlete: {AthleteId}", request.AthleteId);

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
