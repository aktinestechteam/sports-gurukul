using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateRankings;

public class GenerateRankingsCommand : IRequest<Result<IReadOnlyList<RankingDto>>>
{
    public Guid TournamentId { get; set; }
    public Guid? CategoryId { get; set; }
}
