using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Queries.SearchEvents;

public class SearchEventsQuery : IRequest<Result<PagedResult<EventSummaryDto>>>
{
    public Guid? AcademyId { get; set; }
    public Guid? SportId { get; set; }
    public EventStatus? Status { get; set; }
    public EventType? EventType { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
