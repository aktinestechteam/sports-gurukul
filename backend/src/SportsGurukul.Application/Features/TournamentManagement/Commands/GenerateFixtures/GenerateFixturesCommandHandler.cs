using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;

public class GenerateFixturesCommandHandler : IRequestHandler<GenerateFixturesCommand, Result<IReadOnlyList<FixtureDto>>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IFixtureGenerationService _fixtureGenerationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GenerateFixturesCommandHandler> _logger;

    public GenerateFixturesCommandHandler(
        ITournamentRepository tournamentRepository,
        IFixtureGenerationService fixtureGenerationService,
        IUnitOfWork unitOfWork,
        ILogger<GenerateFixturesCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _fixtureGenerationService = fixtureGenerationService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<FixtureDto>>> Handle(GenerateFixturesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating fixtures for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<IReadOnlyList<FixtureDto>>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.RegistrationClosed)
            return Result<IReadOnlyList<FixtureDto>>.Failure("Fixtures can only be generated after registration closes.");

        var details = await _tournamentRepository.GetWithDetailsAsync(request.TournamentId, cancellationToken);
        var participants = details?.Participants?.Where(p => p.IsActive).ToList() ?? [];
        var stages = details?.Stages?.ToList() ?? [];

        if (participants.Count == 0)
            return Result<IReadOnlyList<FixtureDto>>.Failure("No active participants found.");

        var fixtures = await _fixtureGenerationService.GenerateFixturesAsync(tournament, participants, stages, cancellationToken);

        tournament.Status = TournamentStatus.FixtureGeneration;
        _tournamentRepository.Update(tournament);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Fixtures generated for tournament: {TournamentId}, Count: {Count}", tournament.Id, fixtures.Count);

        var dtos = fixtures.Select(f => new FixtureDto
        {
            Id = f.Id,
            TournamentId = f.TournamentId,
            FixtureNumber = f.FixtureNumber,
            ScheduledDate = f.ScheduledDate,
            ScheduledTime = f.ScheduledTime,
            HomeTeamName = f.HomeTeamName,
            AwayTeamName = f.AwayTeamName,
            IsPublished = f.IsPublished,
            Notes = f.Notes,
            CreatedAt = f.CreatedAt
        }).ToList();

        return Result<IReadOnlyList<FixtureDto>>.Success(dtos);
    }
}
