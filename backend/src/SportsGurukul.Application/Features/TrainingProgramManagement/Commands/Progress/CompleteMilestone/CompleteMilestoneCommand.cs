using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.CompleteMilestone;

public record CompleteMilestoneCommand : IRequest<Result<bool>>
{
    public Guid ProgramId { get; init; }
    public Guid MilestoneId { get; init; }
}
