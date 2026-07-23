using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSports;

public class GetAthleteSportsQuery : IRequest<Result<IReadOnlyList<SportDto>>>
{
    public Guid AthleteId { get; set; }
}