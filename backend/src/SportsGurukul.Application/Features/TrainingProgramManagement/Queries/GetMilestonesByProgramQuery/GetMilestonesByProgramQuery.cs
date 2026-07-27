using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;

namespace SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetMilestonesByProgramQuery;

public class GetMilestonesByProgramQuery : IRequest<Result<IReadOnlyList<TrainingMilestoneDto>>>
{
    public Guid ProgramId { get; set; }
}
