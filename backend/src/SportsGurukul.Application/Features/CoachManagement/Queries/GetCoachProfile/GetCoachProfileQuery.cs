using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachProfile;

public class GetCoachProfileQuery : IRequest<Result<CoachProfileDto>>
{
    public Guid CoachId { get; set; }
}
