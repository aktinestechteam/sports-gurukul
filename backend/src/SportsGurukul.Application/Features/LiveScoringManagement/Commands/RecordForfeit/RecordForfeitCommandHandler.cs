using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Competition.Interfaces;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.RecordForfeit;

public class RecordForfeitCommandHandler : IRequestHandler<RecordForfeitCommand, Result<Unit>>
{
    private readonly IMatchLifecycleService _lifecycleService;
    private readonly ILogger<RecordForfeitCommandHandler> _logger;

    public RecordForfeitCommandHandler(IMatchLifecycleService lifecycleService, ILogger<RecordForfeitCommandHandler> logger)
    {
        _lifecycleService = lifecycleService;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordForfeitCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording forfeit for match {MatchId}", request.MatchId);
        await _lifecycleService.RecordForfeitAsync(request.MatchId, request.WinnerId, request.WinnerName, cancellationToken);
        return Result<Unit>.Success(Unit.Value);
    }
}
