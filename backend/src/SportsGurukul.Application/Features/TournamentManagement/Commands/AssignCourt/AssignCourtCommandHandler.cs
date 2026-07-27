using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AssignCourt;

public class AssignCourtCommandHandler : IRequestHandler<AssignCourtCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssignCourtCommandHandler> _logger;

    public AssignCourtCommandHandler(
        IMatchRepository matchRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssignCourtCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(AssignCourtCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning court to match: {MatchId}", request.MatchId);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        match.TournamentCourtId = request.TournamentCourtId;
        _matchRepository.Update(match);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Court assigned to match: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
