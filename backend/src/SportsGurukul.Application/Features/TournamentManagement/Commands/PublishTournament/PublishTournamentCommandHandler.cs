using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.PublishTournament;

public class PublishTournamentCommandHandler : IRequestHandler<PublishTournamentCommand, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishTournamentCommandHandler> _logger;

    public PublishTournamentCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishTournamentCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(PublishTournamentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<TournamentDto>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Draft)
            return Result<TournamentDto>.Failure("Only draft tournaments can be published.");

        tournament.Status = TournamentStatus.Published;
        tournament.IsPublished = true;

        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Tournament published: {TournamentId}", tournament.Id);

        var dto = CreateTournamentCommandHandler.MapToDto(tournament);
        return Result<TournamentDto>.Success(dto);
    }
}
