using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Commands.DeleteSavedSearch;

public class DeleteSavedSearchCommandHandler : IRequestHandler<DeleteSavedSearchCommand, Result<bool>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<DeleteSavedSearchCommandHandler> _logger;

    public DeleteSavedSearchCommandHandler(
        IEventSearchRepository searchRepository,
        ILogger<DeleteSavedSearchCommandHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(DeleteSavedSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting saved search {SearchId} for user {UserId}", request.SavedSearchId, request.UserId);

        try
        {
            await _searchRepository.DeleteSavedSearchAsync(request.SavedSearchId, request.UserId, cancellationToken);
            return Result<bool>.Success(true);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to delete saved search {SearchId}", request.SavedSearchId);
            return Result<bool>.Failure(ex.Message);
        }
    }
}
