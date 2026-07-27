using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateParticipantNumbers;

public class GenerateParticipantNumbersCommandHandler : IRequestHandler<GenerateParticipantNumbersCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly ISeedingService _seedingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateParticipantNumbersCommandHandler> _logger;

    public GenerateParticipantNumbersCommandHandler(
        ITournamentRepository tournamentRepository,
        IRegistrationRepository registrationRepository,
        ISeedingService seedingService,
        IUnitOfWork unitOfWork,
        ILogger<GenerateParticipantNumbersCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _registrationRepository = registrationRepository;
        _seedingService = seedingService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(GenerateParticipantNumbersCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating participant numbers for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        var registrations = await _registrationRepository.GetByTournamentIdAsync(request.TournamentId, cancellationToken);
        var approvedRegistrations = registrations.Where(r => r.RegistrationStatus == TournamentRegistrationStatus.Approved).ToList();

        if (approvedRegistrations.Count == 0)
            return Result<Unit>.Failure("No approved registrations found.");

        var participants = await _tournamentRepository.GetWithDetailsAsync(request.TournamentId, cancellationToken);
        var activeParticipants = participants?.Participants?.Where(p => p.IsActive).ToList() ?? [];

        if (activeParticipants.Count > 0)
        {
            await _seedingService.GenerateSeedsAsync(tournament, activeParticipants, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Participant numbers generated for tournament: {TournamentId}, Count: {Count}", request.TournamentId, approvedRegistrations.Count);
        return Result<Unit>.Success(Unit.Value);
    }
}
