using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RecordCoachRecentSearch;

public class RecordCoachRecentSearchCommandHandler : IRequestHandler<RecordCoachRecentSearchCommand, Result<Unit>>
{
    private readonly IRecentSearchRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RecordCoachRecentSearchCommandHandler> _logger;

    private const int MaxRecentSearches = 20;

    public RecordCoachRecentSearchCommandHandler(
        IRecentSearchRepository repository,
        IUnitOfWork unitOfWork,
        ILogger<RecordCoachRecentSearchCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RecordCoachRecentSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording recent coach search for user: {UserId}", request.UserId);

        var recentSearch = new RecentSearch
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            QueryText = request.QueryText,
            FiltersJson = request.FiltersJson,
            ResultCount = request.ResultCount,
            SearchedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(recentSearch, cancellationToken);
        await _repository.DeleteOlderThanAsync(request.UserId, MaxRecentSearches, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
