using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteRanking;

public class GetAthleteRankingQuery : IRequest<Result<RankingDto>>
{
    public Guid AthleteId { get; set; }
}
