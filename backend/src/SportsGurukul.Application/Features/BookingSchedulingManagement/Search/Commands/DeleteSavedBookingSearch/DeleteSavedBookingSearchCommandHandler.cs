using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.DeleteSavedBookingSearch;

public class DeleteSavedBookingSearchCommandHandler
    : IRequestHandler<DeleteSavedBookingSearchCommand, Result<Unit>>
{
    private readonly ISavedSearchRepository _savedSearchRepository;
    private readonly ILogger<DeleteSavedBookingSearchCommandHandler> _logger;

    public DeleteSavedBookingSearchCommandHandler(
        ISavedSearchRepository savedSearchRepository,
        ILogger<DeleteSavedBookingSearchCommandHandler> logger)
    {
        _savedSearchRepository = savedSearchRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        DeleteSavedBookingSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Deleting saved search {SearchId} for user {UserId}",
            request.SavedSearchId, request.UserId);

        var entity = await _savedSearchRepository.GetByIdAndUserAsync(
            request.SavedSearchId, request.UserId, cancellationToken);

        if (entity is null)
        {
            return Result<Unit>.Failure("Saved search not found or does not belong to this user.");
        }

        _savedSearchRepository.Remove(entity);

        return Result<Unit>.Success(Unit.Value);
    }
}
