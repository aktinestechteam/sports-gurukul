using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CancelTournament;

public class CancelTournamentCommandHandler : IRequestHandler<CancelTournamentCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CancelTournamentCommandHandler> _logger;

    public CancelTournamentCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelTournamentCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(CancelTournamentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cancelling tournament: {TournamentId}, Reason: {Reason}", request.TournamentId, request.Reason);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        var cancellableStatuses = new[] { TournamentStatus.Draft, TournamentStatus.Published, TournamentStatus.RegistrationOpen, TournamentStatus.RegistrationClosed };
        if (!cancellableStatuses.Contains(tournament.Status))
            return Result<Unit>.Failure($"Tournament cannot be cancelled in {tournament.Status} status.");

        tournament.Status = TournamentStatus.Archived;
        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tournament cancelled: {TournamentId}", tournament.Id);
        return Result<Unit>.Success(Unit.Value);
    }
}
