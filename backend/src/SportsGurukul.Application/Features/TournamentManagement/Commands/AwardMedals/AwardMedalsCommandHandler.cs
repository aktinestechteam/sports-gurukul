using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AwardMedals;

public class AwardMedalsCommandHandler : IRequestHandler<AwardMedalsCommand, Result<IReadOnlyList<AwardDto>>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRankingRepository _rankingRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AwardMedalsCommandHandler> _logger;

    public AwardMedalsCommandHandler(
        ITournamentRepository tournamentRepository,
        IRankingRepository rankingRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<AwardMedalsCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _rankingRepository = rankingRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AwardDto>>> Handle(AwardMedalsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Awarding medals for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<IReadOnlyList<AwardDto>>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Completed)
            return Result<IReadOnlyList<AwardDto>>.Failure("Medals can only be awarded for completed tournaments.");

        var topRankings = await _rankingRepository.GetTopRankingsAsync(request.TournamentId, 3, cancellationToken);

        var awards = new List<TournamentAward>();
        var awardTypes = new[] { TournamentAwardType.Winner, TournamentAwardType.RunnerUp, TournamentAwardType.ThirdPlace };

        for (int i = 0; i < Math.Min(topRankings.Count, 3); i++)
        {
            var award = new TournamentAward
            {
                Id = Guid.NewGuid(),
                TournamentId = request.TournamentId,
                AwardType = awardTypes[i],
                AwardName = awardTypes[i].ToString(),
                ParticipantId = topRankings[i].ParticipantId,
                Description = $"Rank {topRankings[i].Rank} finish"
            };
            awards.Add(award);
            _context.TournamentAwards.Add(award);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Medals awarded for tournament: {TournamentId}, Count: {Count}", tournament.Id, awards.Count);

        var dtos = awards.Select(a => new AwardDto
        {
            Id = a.Id,
            TournamentId = a.TournamentId,
            AwardType = a.AwardType,
            AwardName = a.AwardName,
            ParticipantId = a.ParticipantId,
            Description = a.Description,
            CreatedAt = a.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<AwardDto>>.Success(dtos);
    }
}
