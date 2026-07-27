using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.PublishResults;

public class PublishResultsCommandHandler : IRequestHandler<PublishResultsCommand, Result<Unit>>
{
    private readonly ITournamentRepository _tournamentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PublishResultsCommandHandler> _logger;

    public PublishResultsCommandHandler(
        ITournamentRepository tournamentRepository,
        IUnitOfWork unitOfWork,
        ILogger<PublishResultsCommandHandler> logger)
    {
        _tournamentRepository = tournamentRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(PublishResultsCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Publishing results for tournament: {TournamentId}", request.TournamentId);

        var tournament = await _tournamentRepository.GetByIdAsync(request.TournamentId, cancellationToken);
        if (tournament is null)
            return Result<Unit>.Failure("Tournament not found.");

        if (tournament.Status != TournamentStatus.Completed)
            return Result<Unit>.Failure("Results can only be published for completed tournaments.");

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Results published for tournament: {TournamentId}", tournament.Id);
        return Result<Unit>.Success(Unit.Value);
    }
}
