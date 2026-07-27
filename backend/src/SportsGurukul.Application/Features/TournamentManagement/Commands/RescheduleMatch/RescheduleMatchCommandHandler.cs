using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RescheduleMatch;

public class RescheduleMatchCommandHandler : IRequestHandler<RescheduleMatchCommand, Result<Unit>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RescheduleMatchCommandHandler> _logger;

    public RescheduleMatchCommandHandler(
        IMatchRepository matchRepository,
        IUnitOfWork unitOfWork,
        ILogger<RescheduleMatchCommandHandler> logger)
    {
        _matchRepository = matchRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RescheduleMatchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rescheduling match: {MatchId} to {NewDate} {NewTime}", request.MatchId, request.NewDate, request.NewTime);

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);
        if (match is null)
            return Result<Unit>.Failure("Match not found.");

        if (match.Status != MatchStatus.Scheduled)
            return Result<Unit>.Failure("Only scheduled matches can be rescheduled.");

        match.ScheduledDate = request.NewDate;
        match.ScheduledTime = request.NewTime;
        match.Notes = request.Reason;

        _matchRepository.Update(match);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Match rescheduled: {MatchId}", request.MatchId);
        return Result<Unit>.Success(Unit.Value);
    }
}
