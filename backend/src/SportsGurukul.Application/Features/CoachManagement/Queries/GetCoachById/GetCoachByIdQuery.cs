using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachById;

public class GetCoachByIdQuery : IRequest<Result<CoachDto>>
{
    public Guid CoachId { get; set; }
}
