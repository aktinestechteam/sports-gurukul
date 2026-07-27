using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.ArchiveTournament;

public class ArchiveTournamentCommandHandler : IRequestHandler<ArchiveTournamentCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ArchiveTournamentCommandHandler> _logger;

    public ArchiveTournamentCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<ArchiveTournamentCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ArchiveTournamentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Archiving tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Completed)
            return Result<Unit>.Failure("Only completed tournaments can be archived.");

        tournament.Status = TournamentStatus.Archived;
        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tournament archived: {TournamentId}", tournament.Id);
        return Result<Unit>.Success(Unit.Value);
    }
}
