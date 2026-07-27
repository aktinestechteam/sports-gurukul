using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Commands.RecordBookingSearch;

public class RecordBookingSearchCommandHandler
    : IRequestHandler<RecordBookingSearchCommand, Result<Unit>>
{
    private readonly IRecentSearchRepository _recentSearchRepository;
    private readonly ILogger<RecordBookingSearchCommandHandler> _logger;

    private const int MaxRecentSearches = 20;

    public RecordBookingSearchCommandHandler(
        IRecentSearchRepository recentSearchRepository,
        ILogger<RecordBookingSearchCommandHandler> logger)
    {
        _recentSearchRepository = recentSearchRepository;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(
        RecordBookingSearchCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Recording booking search for user {UserId}: '{SearchTerm}'",
            request.UserId, request.SearchTerm);

        var filters = new BookingSearchFilterDto
        {
            SearchTerm = request.SearchTerm,
            AcademyId = request.AcademyId,
            FacilityId = request.FacilityId,
            BookingType = request.BookingType,
            Status = request.Status
        };

        var entity = new RecentSearch
        {
            UserId = request.UserId,
            QueryText = request.SearchTerm,
            ResultCount = request.ResultCount,
            SearchedAt = DateTime.UtcNow
        };
        entity.SetFilters(filters);

        await _recentSearchRepository.AddAsync(entity, cancellationToken);

        await _recentSearchRepository.DeleteOlderThanAsync(
            request.UserId, MaxRecentSearches, cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}
