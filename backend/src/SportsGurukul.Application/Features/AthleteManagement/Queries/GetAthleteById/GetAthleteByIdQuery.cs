using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteById;

public class GetAthleteByIdQuery : IRequest<Result<AthleteDto>>
{
    public Guid AthleteId { get; set; }
}