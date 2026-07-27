using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;

public record CreateAssessmentCommand : IRequest<Result<DTOs.AssessmentDto>>
{
    public Guid SessionId { get; init; }
    public string AssessmentType { get; init; } = string.Empty;
    public string AssessmentName { get; init; } = string.Empty;
    public decimal MaximumScore { get; init; }
    public decimal PassingScore { get; init; }
}
