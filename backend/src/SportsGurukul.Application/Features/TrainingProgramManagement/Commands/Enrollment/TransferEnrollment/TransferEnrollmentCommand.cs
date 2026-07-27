using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;

public record TransferEnrollmentCommand : IRequest<Result<DTOs.EnrollmentDto>>
{
    public Guid EnrollmentId { get; init; }
    public Guid SourceBatchId { get; init; }
    public Guid TargetBatchId { get; init; }
}
