using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;

public class DeleteAchievementCommand : IRequest<Result<Unit>>
{
    public Guid AthleteId { get; set; }
    public Guid AchievementId { get; set; }
}
