using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.Queries.CalendarView;

public class CalendarViewQuery : IRequest<Result<CalendarViewResultDto>>
{
    public Guid AcademyId { get; set; }
    public CalendarViewType ViewType { get; set; } = CalendarViewType.Monthly;
    public DateTime? ViewDate { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
}
