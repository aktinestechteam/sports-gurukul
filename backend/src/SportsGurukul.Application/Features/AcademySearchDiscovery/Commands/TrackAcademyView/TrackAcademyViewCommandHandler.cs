using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.TrackAcademyView;

public class TrackAcademyViewCommandHandler : IRequestHandler<TrackAcademyViewCommand, Result<Unit>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<TrackAcademyViewCommandHandler> _logger;

    public TrackAcademyViewCommandHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<TrackAcademyViewCommandHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(TrackAcademyViewCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tracking academy view: AcademyId={AcademyId}, Source={Source}", request.AcademyId, request.Source);

        var view = new AcademyView
        {
            Id = Guid.NewGuid(),
            AcademyId = request.AcademyId,
            ViewedByUserId = request.UserId,
            ViewedAt = DateTime.UtcNow,
            Source = request.Source
        };

        await _academySearchRepository.TrackViewAsync(view, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
