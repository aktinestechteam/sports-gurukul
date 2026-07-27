using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.MarkAttendance;

public record MarkAttendanceCommand : IRequest<Result<DTOs.AttendanceDto>>
{
    public Guid SessionId { get; init; }
    public Guid AthleteId { get; init; }
    public AttendanceStatus Status { get; init; }
    public string? Remarks { get; init; }
}
