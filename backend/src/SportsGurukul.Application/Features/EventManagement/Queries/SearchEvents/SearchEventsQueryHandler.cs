using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.SearchEvents;

public class SearchEventsQueryHandler : IRequestHandler<SearchEventsQuery, Result<PagedResult<EventSummaryDto>>>
{
    private readonly IEventRepository _eventRepository;
    private readonly ILogger<SearchEventsQueryHandler> _logger;

    public SearchEventsQueryHandler(
        IEventRepository eventRepository,
        ILogger<SearchEventsQueryHandler> logger)
    {
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<EventSummaryDto>>> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Searching events: AcademyId={AcademyId}, SportId={SportId}, Status={Status}, Page={Page}, PageSize={PageSize}",
            request.AcademyId, request.SportId, request.Status, request.Page, request.PageSize);

        var events = await _eventRepository.SearchAsync(
            request.AcademyId,
            request.SportId,
            request.Status,
            request.EventType,
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        var totalCount = await _eventRepository.CountSearchAsync(
            request.AcademyId,
            request.SportId,
            request.Status,
            request.EventType,
            request.SearchTerm,
            cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var items = events.Select(e => new EventSummaryDto
        {
            Id = e.Id,
            EventCode = e.EventCode,
            EventName = e.EventName,
            ShortDescription = e.ShortDescription,
            Status = e.Status.ToString(),
            StartDate = e.StartDate,
            EndDate = e.EndDate,
            MaxParticipants = e.MaxParticipants,
            RegistrationFee = e.RegistrationFee,
            IsFeatured = e.IsFeatured,
            BannerUrl = e.BannerUrl
        }).ToList();

        var result = new PagedResult<EventSummaryDto>
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<PagedResult<EventSummaryDto>>.Success(result);
    }
}
