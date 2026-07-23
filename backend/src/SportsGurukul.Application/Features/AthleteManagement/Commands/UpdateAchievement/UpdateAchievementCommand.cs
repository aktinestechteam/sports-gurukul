using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;

public class UpdateAchievementCommand : IRequest<Result<AthleteAchievementDto>>
{
    public Guid AthleteId { get; set; }
    public Guid AchievementId { get; set; }
    public string? Title { get; set; }
    public string? Competition { get; set; }
    public string? Position { get; set; }
    public AchievementLevel? Level { get; set; }
    public DateTime? Date { get; set; }
    public string? CertificateUrl { get; set; }
    public string? Notes { get; set; }
}
