using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;

public class SearchEventsQueryHandler : IRequestHandler<SearchEventsQuery, Result<EventSearchPageResultDto>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ILogger<SearchEventsQueryHandler> _logger;

    public SearchEventsQueryHandler(
        IEventSearchRepository searchRepository,
        ILogger<SearchEventsQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _logger = logger;
    }

    public async Task<Result<EventSearchPageResultDto>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Searching events: Term='{Term}', Page={Page}", request.SearchTerm, request.Page);

        var events = await _searchRepository.SearchEventsAsync(
            request.SearchTerm, request.SportId, request.AcademyId, request.CoachId,
            request.EventType, request.Category, request.SkillLevel, request.AgeGroup,
            request.City, request.State, request.Country,
            request.DateFrom, request.DateTo,
            request.MinPrice, request.MaxPrice,
            request.MinRating, request.Language,
            request.Availability, request.RegistrationStatus,
            request.SortBy, request.SortDescending,
            request.Page, request.PageSize, cancellationToken);

        var totalCount = await _searchRepository.CountSearchEventsAsync(
            request.SearchTerm, request.SportId, request.AcademyId, request.CoachId,
            request.EventType, request.Category, request.SkillLevel, request.AgeGroup,
            request.City, request.State, request.Country,
            request.DateFrom, request.DateTo,
            request.MinPrice, request.MaxPrice,
            request.MinRating, request.Language,
            request.Availability, request.RegistrationStatus, cancellationToken);

        stopwatch.Stop();

        var items = events.Select(MapToEventCardDto).ToList();
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        return Result<EventSearchPageResultDto>.Success(new EventSearchPageResultDto
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            SearchTimeMs = stopwatch.Elapsed.TotalMilliseconds
        });
    }

    private static EventCardDto MapToEventCardDto(Domain.Entities.Event evt)
    {
        var now = DateTime.UtcNow;
        var daysUntilStart = (evt.StartDate - now).Days;
        var isRegOpen = evt.Status == Domain.Enums.EventStatus.RegistrationOpen &&
                        evt.RegistrationCloseDate >= now;

        return new EventCardDto
        {
            Id = evt.Id,
            EventCode = evt.EventCode,
            EventName = evt.EventName,
            ShortDescription = evt.ShortDescription,
            BannerUrl = evt.BannerUrl,
            Status = evt.Status.ToString(),
            StartDate = evt.StartDate,
            EndDate = evt.EndDate,
            RegistrationCloseDate = evt.RegistrationCloseDate,
            MaxParticipants = evt.MaxParticipants,
            RegistrationFee = evt.RegistrationFee,
            IsFeatured = evt.IsFeatured,
            IsPublic = evt.IsPublic,
            SportName = evt.Sport?.Name,
            City = evt.Venues?.FirstOrDefault(v => v.IsPrimary)?.City,
            State = evt.Venues?.FirstOrDefault(v => v.IsPrimary)?.State,
            Latitude = evt.Venues?.FirstOrDefault(v => v.IsPrimary)?.Latitude,
            Longitude = evt.Venues?.FirstOrDefault(v => v.IsPrimary)?.Longitude,
            AverageRating = evt.Feedbacks?.Any() == true
                ? (decimal)evt.Feedbacks.Average(f => (int)f.OverallRating)
                : 0,
            TotalReviews = evt.Feedbacks?.Count ?? 0,
            DaysUntilStart = daysUntilStart,
            IsRegistrationOpen = isRegOpen,
            IsSoldOut = evt.MaxParticipants.HasValue &&
                        evt.Registrations?.Count(r => r.Status == Domain.Enums.EventRegistrationStatus.Approved) >= evt.MaxParticipants.Value
        };
    }
}
