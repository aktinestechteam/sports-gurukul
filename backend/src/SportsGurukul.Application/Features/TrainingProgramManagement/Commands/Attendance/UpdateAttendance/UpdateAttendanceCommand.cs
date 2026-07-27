using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.UpdateAttendance;

public record UpdateAttendanceCommand : IRequest<Result<DTOs.AttendanceDto>>
{
    public Guid AttendanceId { get; init; }
    public AttendanceStatus Status { get; init; }
    public string? Remarks { get; init; }
}
