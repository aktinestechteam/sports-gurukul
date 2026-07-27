using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordWalkover;

public class RecordWalkoverCommandHandler : IRequestHandler<RecordWalkoverCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<RecordWalkoverCommandHandler> _logger;

    public RecordWalkoverCommandHandler(IMatchLifecycleService lifecycleService, ILogger<RecordWalkoverCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordWalkoverCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording walkover for match {MatchId}", request.MatchId);
        await _lifecycleService.RecordWalkoverAsync(request.MatchId, request.WinnerId, request.WinnerName, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
