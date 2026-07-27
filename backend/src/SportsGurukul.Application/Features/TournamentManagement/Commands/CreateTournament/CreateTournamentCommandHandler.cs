using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;

public class CreateTournamentCommandHandler : IRequestHandler<CreateTournamentCommand, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTournamentCommandHandler> _logger;

    public CreateTournamentCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateTournamentCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(CreateTournamentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating tournament: {TournamentName}", request.TournamentName);

        if (request.EndDate <= request.StartDate)
            return Result<TournamentDto>.Failure("End date must be after start date.");

        if (request.RegistrationCloseDate >= request.StartDate)
            return Result<TournamentDto>.Failure("Registration must close before the tournament starts.");

        var tournamentCode = $"TRN-{DateTime.UtcNow:yyyyMMddHHmmss}";
        var tournament = new Domain.Entities.Tournament
        {
            Id = Guid.NewGuid(),
            TournamentCode = tournamentCode,
            TournamentName = request.TournamentName,
            Description = request.Description,
            AcademyId = request.AcademyId,
            SportId = request.SportId,
            TournamentType = request.TournamentType,
            Status = TournamentStatus.Draft,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            RegistrationOpenDate = request.RegistrationOpenDate,
            RegistrationCloseDate = request.RegistrationCloseDate,
            MaxParticipants = request.MaxParticipants,
            MinParticipants = request.MinParticipants,
            RegistrationFee = request.RegistrationFee,
            RegistrationType = request.RegistrationType,
            Venue = request.Venue,
            Rules = request.Rules,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            Website = request.Website,
            IsPublished = false
        };

        await _tournamentRepository.AddAsync(tournament, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tournament created: {TournamentId}, Code: {TournamentCode}", tournament.Id, tournamentCode);

        var dto = MapToDto(tournament);
        return Result<TournamentDto>.Success(dto);
    }

    internal static TournamentDto MapToDto(Domain.Entities.Tournament tournament)
    {
        return new TournamentDto
        {
            Id = tournament.Id,
            TournamentCode = tournament.TournamentCode,
            TournamentName = tournament.TournamentName,
            Description = tournament.Description,
            AcademyId = tournament.AcademyId,
            SportId = tournament.SportId,
            TournamentType = tournament.TournamentType,
            Status = tournament.Status,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            RegistrationOpenDate = tournament.RegistrationOpenDate,
            RegistrationCloseDate = tournament.RegistrationCloseDate,
            MaxParticipants = tournament.MaxParticipants,
            MinParticipants = tournament.MinParticipants,
            RegistrationFee = tournament.RegistrationFee,
            RegistrationType = tournament.RegistrationType,
            Venue = tournament.Venue,
            Rules = tournament.Rules,
            ContactEmail = tournament.ContactEmail,
            ContactPhone = tournament.ContactPhone,
            Website = tournament.Website,
            IsPublished = tournament.IsPublished,
            CreatedAt = tournament.CreatedAt,
            UpdatedAt = tournament.UpdatedAt
        };
    }
}
