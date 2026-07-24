using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachByUserId;

public class GetCoachByUserIdQuery : IRequest<Result<CoachDto>>
{
    public Guid UserId { get; set; }
}
