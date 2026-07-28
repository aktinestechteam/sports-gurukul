using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.EventManagement.Commands.MarkAttendance;

public class MarkAttendanceCommand : IRequest<Result<AttendanceDto>>
{
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid ParticipantId { get; set; }
    public EventAttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
