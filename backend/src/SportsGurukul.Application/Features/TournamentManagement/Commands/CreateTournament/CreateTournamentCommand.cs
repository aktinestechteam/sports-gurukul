using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;

public class CreateTournamentCommand : IRequest<Result<TournamentDto>>
{
    public string TournamentName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public TournamentType TournamentType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public RegistrationType RegistrationType { get; set; }
    public string? Venue { get; set; }
    public string? Rules { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
}
