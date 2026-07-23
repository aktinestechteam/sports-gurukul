using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;

public class AddAchievementCommand : IRequest<Result<AthleteAchievementDto>>
{
    public Guid AthleteId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Competition { get; set; }
    public string? Position { get; set; }
    public AchievementLevel Level { get; set; } = AchievementLevel.Local;
    public DateTime Date { get; set; }
    public string? CertificateUrl { get; set; }
    public string? Notes { get; set; }
}
