using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetRecentBookingSearches;

public class GetRecentBookingSearchesQueryHandler
    : IRequestHandler<GetRecentBookingSearchesQuery, Result<IReadOnlyList<RecentBookingSearchDto>>>
{
    private readonly IRecentSearchRepository _recentSearchRepository;
    private readonly ILogger<GetRecentBookingSearchesQueryHandler> _logger;

    public GetRecentBookingSearchesQueryHandler(
        IRecentSearchRepository recentSearchRepository,
        ILogger<GetRecentBookingSearchesQueryHandler> logger)
    {
        _recentSearchRepository = recentSearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<RecentBookingSearchDto>>> Handle(
        GetRecentBookingSearchesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting recent booking searches for user {UserId}, limit={Limit}",
            request.UserId, request.Limit);

        var searches = await _recentSearchRepository.GetByUserIdAsync(
            request.UserId, request.Limit, cancellationToken);

        var dtos = searches.Select(s =>
        {
            var filters = s.GetFilters<BookingSearchFilterDto>();
            return new RecentBookingSearchDto
            {
                Id = s.Id,
                QueryText = s.QueryText,
                Filters = filters,
                ResultCount = s.ResultCount,
                SearchedAt = s.SearchedAt
            };
        }).ToList();

        return Result<IReadOnlyList<RecentBookingSearchDto>>.Success(dtos);
    }
}
