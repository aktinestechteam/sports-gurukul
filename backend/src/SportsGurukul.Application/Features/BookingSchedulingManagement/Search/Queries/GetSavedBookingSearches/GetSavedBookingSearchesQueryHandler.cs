using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetSavedBookingSearches;

public class GetSavedBookingSearchesQueryHandler
    : IRequestHandler<GetSavedBookingSearchesQuery, Result<IReadOnlyList<SavedBookingSearchDto>>>
{
    private readonly ISavedSearchRepository _savedSearchRepository;
    private readonly ILogger<GetSavedBookingSearchesQueryHandler> _logger;

    public GetSavedBookingSearchesQueryHandler(
        ISavedSearchRepository savedSearchRepository,
        ILogger<GetSavedBookingSearchesQueryHandler> logger)
    {
        _savedSearchRepository = savedSearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<SavedBookingSearchDto>>> Handle(
        GetSavedBookingSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting saved booking searches for user {UserId}", request.UserId);

        var searches = await _savedSearchRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var dtos = searches.Select(s =>
        {
            var filters = s.GetFilters<BookingSearchFilterDto>() ?? new BookingSearchFilterDto();
            return new SavedBookingSearchDto
            {
                Id = s.Id,
                Name = s.Name,
                Filters = filters,
                UsageCount = s.UsageCount,
                CreatedAt = s.CreatedAt
            };
        }).ToList();

        return Result<IReadOnlyList<SavedBookingSearchDto>>.Success(dtos);
    }
}
