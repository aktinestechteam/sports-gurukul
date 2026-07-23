using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;

public class UpdateRankingCommand : IRequest<Result<RankingDto>>
{
    public Guid AthleteId { get; set; }
    public string? CurrentRank { get; set; }
    public string? StateRank { get; set; }
    public string? NationalRank { get; set; }
    public string? InternationalRank { get; set; }
    public string? RankingAuthority { get; set; }
}
