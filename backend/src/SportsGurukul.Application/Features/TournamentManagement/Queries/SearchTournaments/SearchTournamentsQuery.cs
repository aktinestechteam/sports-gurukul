using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;

public class SearchTournamentsQuery : IRequest<Result<TournamentSearchResponse>>
{
    public Guid? AcademyId { get; set; }
    public TournamentStatus? Status { get; set; }
    public TournamentType? TournamentType { get; set; }
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
