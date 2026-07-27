using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.GetResourceCalendar;

public class GetResourceCalendarQuery : IRequest<Result<CalendarViewResultDto>>
{
    public Guid AcademyId { get; set; }
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
