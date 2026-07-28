using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.CalendarEvents;

public class CalendarEventsQuery : IRequest<Result<IReadOnlyList<CalendarEventDto>>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? AcademyId { get; set; }
    public CalendarViewType ViewType { get; set; } = CalendarViewType.Monthly;
}
