using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteByUserId;

public class GetAthleteByUserIdQuery : IRequest<Result<AthleteDto>>
{
    public Guid UserId { get; set; }
}