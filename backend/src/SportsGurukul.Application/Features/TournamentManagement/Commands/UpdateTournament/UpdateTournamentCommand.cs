using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;

public class UpdateTournamentCommand : IRequest<Result<TournamentDto>>
{
    public Guid TournamentId { get; set; }
    public string? TournamentName { get; set; }
    public string? Description { get; set; }
    public TournamentType? TournamentType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? RegistrationOpenDate { get; set; }
    public DateTime? RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public RegistrationType? RegistrationType { get; set; }
    public string? Venue { get; set; }
    public string? Rules { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
}
