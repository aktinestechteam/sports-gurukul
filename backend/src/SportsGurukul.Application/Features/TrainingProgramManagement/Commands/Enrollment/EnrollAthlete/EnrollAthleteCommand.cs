using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.EnrollAthlete;

public record EnrollAthleteCommand : IRequest<Result<DTOs.EnrollmentDto>>
{
    public Guid BatchId { get; init; }
    public Guid AthleteId { get; init; }
}
