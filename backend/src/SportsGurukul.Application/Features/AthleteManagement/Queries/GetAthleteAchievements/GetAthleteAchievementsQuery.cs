using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteAchievements;

public class GetAthleteAchievementsQuery : IRequest<Result<IReadOnlyList<AthleteAchievementDto>>>
{
    public Guid AthleteId { get; set; }
}
