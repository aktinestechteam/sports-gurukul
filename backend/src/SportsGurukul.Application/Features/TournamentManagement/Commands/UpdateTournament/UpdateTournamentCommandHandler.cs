using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;

public class UpdateTournamentCommandHandler : IRequestHandler<UpdateTournamentCommand, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateTournamentCommandHandler> _logger;

    public UpdateTournamentCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<UpdateTournamentCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(UpdateTournamentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<TournamentDto>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Draft)
            return Result<TournamentDto>.Failure("Tournament can only be updated in Draft status.");

        if (request.TournamentName is not null) tournament.TournamentName = request.TournamentName;
        if (request.Description is not null) tournament.Description = request.Description;
        if (request.TournamentType.HasValue) tournament.TournamentType = request.TournamentType.Value;
        if (request.StartDate.HasValue) tournament.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue) tournament.EndDate = request.EndDate.Value;
        if (request.RegistrationOpenDate.HasValue) tournament.RegistrationOpenDate = request.RegistrationOpenDate.Value;
        if (request.RegistrationCloseDate.HasValue) tournament.RegistrationCloseDate = request.RegistrationCloseDate.Value;
        if (request.MaxParticipants.HasValue) tournament.MaxParticipants = request.MaxParticipants;
        if (request.MinParticipants.HasValue) tournament.MinParticipants = request.MinParticipants;
        if (request.RegistrationFee.HasValue) tournament.RegistrationFee = request.RegistrationFee;
        if (request.RegistrationType.HasValue) tournament.RegistrationType = request.RegistrationType.Value;
        if (request.Venue is not null) tournament.Venue = request.Venue;
        if (request.Rules is not null) tournament.Rules = request.Rules;
        if (request.ContactEmail is not null) tournament.ContactEmail = request.ContactEmail;
        if (request.ContactPhone is not null) tournament.ContactPhone = request.ContactPhone;
        if (request.Website is not null) tournament.Website = request.Website;

        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tournament updated: {TournamentId}", tournament.Id);

        var dto = CreateTournamentCommandHandler.MapToDto(tournament);
        return Result<TournamentDto>.Success(dto);
    }
}
