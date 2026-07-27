using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentById;

public class GetTournamentByIdQueryHandler : IRequestHandler<GetTournamentByIdQuery, Result<TournamentDto>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ILogger<GetTournamentByIdQueryHandler> _logger;

    public GetTournamentByIdQueryHandler(
        ITournamentRepository tournamentRepository,
        ILogger<GetTournamentByIdQueryHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _logger = logger;
    }

    public async Task<Result<TournamentDto>> Handle(GetTournamentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetWithDetailsAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<TournamentDto>.Failure("Tournament not found.");

        var dto = CreateTournamentCommandHandler.MapToDto(tournament);
        dto.RegisteredCount = tournament.Registrations?.Count(r => !r.IsDeleted) ?? 0;
        dto.MatchCount = 0;

        return Result<TournamentDto>.Success(dto);
    }
}
