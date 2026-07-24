using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;

public class DeleteExperienceCommand : IRequest<Result<Unit>>
{
    public Guid ExperienceId { get; set; }
}
