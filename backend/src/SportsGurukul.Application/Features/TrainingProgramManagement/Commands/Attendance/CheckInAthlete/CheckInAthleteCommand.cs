using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Attendance.CheckInAthlete;

public record CheckInAthleteCommand : IRequest<Result<DTOs.AttendanceDto>>
{
    public Guid SessionId { get; init; }
    public Guid AthleteId { get; init; }
}
