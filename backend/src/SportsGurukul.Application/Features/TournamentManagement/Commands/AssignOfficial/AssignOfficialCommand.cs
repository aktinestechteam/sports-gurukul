using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.AssignOfficial;

public class AssignOfficialCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
    public Guid? CoachId { get; set; }
    public string OfficialName { get; set; } = string.Empty;
    public TournamentOfficialRole Role { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
