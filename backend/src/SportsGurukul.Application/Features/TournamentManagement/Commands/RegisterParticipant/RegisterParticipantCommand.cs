using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;

public class RegisterParticipantCommand : IRequest<Result<ParticipantDto>>
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
    public TournamentParticipantType ParticipantType { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? TeamId { get; set; }
    public Guid? AcademyId { get; set; }
    public string RegistrantName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Notes { get; set; }
}
