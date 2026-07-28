using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetAttendanceByEvent;

public class GetAttendanceByEventQuery : IRequest<Result<PagedResult<AttendanceDto>>>
{
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public EventAttendanceStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
