using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;

public record SubmitAssessmentResultCommand : IRequest<Result<DTOs.AssessmentResultDto>>
{
    public Guid AssessmentId { get; init; }
    public Guid AthleteId { get; init; }
    public decimal Score { get; init; }
    public string? Remarks { get; init; }
}
