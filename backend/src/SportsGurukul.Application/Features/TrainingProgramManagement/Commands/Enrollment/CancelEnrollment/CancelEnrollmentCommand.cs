using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CancelEnrollment;

public record CancelEnrollmentCommand : IRequest<Result<DTOs.EnrollmentDto>>
{
    public Guid EnrollmentId { get; init; }
    public Guid BatchId { get; init; }
}
