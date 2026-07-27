using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CloseRegistration;

public class CloseRegistrationCommandHandler : IRequestHandler<CloseRegistrationCommand, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CloseRegistrationCommandHandler> _logger;

    public CloseRegistrationCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CloseRegistrationCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(CloseRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Closing registration for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<TournamentDto>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.RegistrationOpen)
            return Result<TournamentDto>.Failure("Registration is not currently open for this tournament.");

        tournament.Status = TournamentStatus.RegistrationClosed;

        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration closed for tournament: {TournamentId}", tournament.Id);

        var dto = CreateTournamentCommandHandler.MapToDto(tournament);
        return Result<TournamentDto>.Success(dto);
    }
}
