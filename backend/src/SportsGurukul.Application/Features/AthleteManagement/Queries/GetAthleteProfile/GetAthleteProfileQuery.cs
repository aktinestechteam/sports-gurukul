using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteProfile;

public class GetAthleteProfileQuery : IRequest<Result<AthleteDto>>
{
    public Guid AthleteId { get; set; }
}