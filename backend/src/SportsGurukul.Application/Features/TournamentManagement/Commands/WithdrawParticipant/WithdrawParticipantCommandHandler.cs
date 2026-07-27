using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.WithdrawParticipant;

public class WithdrawParticipantCommandHandler : IRequestHandler<WithdrawParticipantCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WithdrawParticipantCommandHandler> _logger;

    public WithdrawParticipantCommandHandler(
        ITournamentRepository tournamentRepository,
        IRegistrationRepository registrationRepository,
        IUnitOfWork unitOfWork,
        ILogger<WithdrawParticipantCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _registrationRepository = registrationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(WithdrawParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Withdrawing participant: {ParticipantId} from tournament: {TournamentId}", request.ParticipantId, request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        var withdrawableStatuses = new[] { TournamentStatus.RegistrationOpen, TournamentStatus.RegistrationClosed };
        if (!withdrawableStatuses.Contains(tournament.Status))
            return Result<Unit>.Failure("Participant cannot be withdrawn in current tournament status.");

        var registrations = await _registrationRepository.GetByTournamentIdAsync(request.TournamentId, cancellationToken);
        var registration = registrations.FirstOrDefault(r => r.AthleteId == request.ParticipantId || r.TeamId == request.ParticipantId);
        if (registration is null)
            return Result<Unit>.Failure("Registration not found for this participant.");

        registration.RegistrationStatus = TournamentRegistrationStatus.Cancelled;
        _registrationRepository.Update(registration);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Participant withdrawn: {ParticipantId}", request.ParticipantId);
        return Result<Unit>.Success(Unit.Value);
    }
}
