using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.RecordAcademySearch;

public class RecordAcademySearchCommandHandler : IRequestHandler<RecordAcademySearchCommand, Result<Unit>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<RecordAcademySearchCommandHandler> _logger;

    public RecordAcademySearchCommandHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<RecordAcademySearchCommandHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordAcademySearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording academy search for user: {UserId}", request.UserId);

        var recentSearch = new RecentAcademySearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            SearchTerm = request.SearchTerm,
            City = request.City,
            State = request.State,
            SportName = request.SportName,
            AcademyCount = request.AcademyCount,
            SearchedAt = DateTime.UtcNow
        };

        await _academySearchRepository.RecordSearchAsync(recentSearch, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
