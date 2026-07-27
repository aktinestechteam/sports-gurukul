using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.OpenRegistration;

public class OpenRegistrationCommandHandler : IRequestHandler<OpenRegistrationCommand, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OpenRegistrationCommandHandler> _logger;

    public OpenRegistrationCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<OpenRegistrationCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(OpenRegistrationCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Opening registration for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<TournamentDto>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Published)
            return Result<TournamentDto>.Failure("Registration can only be opened for published tournaments.");

        tournament.Status = TournamentStatus.RegistrationOpen;
        if (request.RegistrationCloseDate.HasValue)
            tournament.RegistrationCloseDate = request.RegistrationCloseDate.Value;

        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Registration opened for tournament: {TournamentId}", tournament.Id);

        var dto = CreateTournamentCommandHandler.MapToDto(tournament);
        return Result<TournamentDto>.Success(dto);
    }
}
