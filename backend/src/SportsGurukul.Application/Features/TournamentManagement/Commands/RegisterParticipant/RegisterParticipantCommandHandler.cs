using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;

public class RegisterParticipantCommandHandler : IRequestHandler<RegisterParticipantCommand, Result<ParticipantDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IRegistrationRepository _registrationRepository;
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RegisterParticipantCommandHandler> _logger;

    public RegisterParticipantCommandHandler(
        ITournamentRepository tournamentRepository,
        IRegistrationRepository registrationRepository,
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<RegisterParticipantCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _registrationRepository = registrationRepository;
        _context = context;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ParticipantDto>> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering participant for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<ParticipantDto>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.RegistrationOpen)
            return Result<ParticipantDto>.Failure("Registration is not open for this tournament.");

        var alreadyRegistered = await _registrationRepository.IsAlreadyRegisteredAsync(
            request.TournamentId, request.AthleteId, request.TeamId, cancellationToken);
        if (alreadyRegistered)
            return Result<ParticipantDto>.Failure("Participant is already registered for this tournament.");

        if (tournament.MaxParticipants.HasValue)
        {
            var currentCount = await _registrationRepository.GetRegistrationCountAsync(request.TournamentId, cancellationToken);
            if (currentCount >= tournament.MaxParticipants.Value)
                return Result<ParticipantDto>.Failure("Tournament has reached maximum participants.");
        }

        var registration = new TournamentRegistration
        {
            Id = Guid.NewGuid(),
            TournamentId = request.TournamentId,
            CategoryId = request.CategoryId,
            RegistrationStatus = TournamentRegistrationStatus.Pending,
            AthleteId = request.AthleteId,
            TeamId = request.TeamId,
            AcademyId = request.AcademyId,
            RegistrantName = request.RegistrantName,
            Email = request.Email,
            Phone = request.Phone,
            Notes = request.Notes
        };

        await _registrationRepository.AddAsync(registration, cancellationToken);

        var participant = new TournamentParticipant
        {
            Id = Guid.NewGuid(),
            TournamentId = request.TournamentId,
            CategoryId = request.CategoryId,
            ParticipantType = request.ParticipantType,
            AthleteId = request.AthleteId,
            TeamId = request.TeamId,
            AcademyId = request.AcademyId,
            ParticipantName = request.RegistrantName,
            IsActive = true
        };

        _context.TournamentParticipants.Add(participant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Participant registered: {ParticipantId} for tournament: {TournamentId}", participant.Id, request.TournamentId);

        var dto = new ParticipantDto
        {
            Id = participant.Id,
            TournamentId = participant.TournamentId,
            CategoryId = participant.CategoryId,
            ParticipantType = participant.ParticipantType,
            AthleteId = participant.AthleteId,
            TeamId = participant.TeamId,
            AcademyId = participant.AcademyId,
            ParticipantName = participant.ParticipantName,
            IsActive = participant.IsActive,
            CreatedAt = participant.CreatedAt
        };

        return Result<ParticipantDto>.Success(dto);
    }
}
