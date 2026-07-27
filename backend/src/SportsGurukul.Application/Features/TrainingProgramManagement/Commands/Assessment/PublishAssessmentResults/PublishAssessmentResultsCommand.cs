using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;

public record PublishAssessmentResultsCommand : IRequest<Result<bool>>
{
    public Guid AssessmentId { get; init; }
}
