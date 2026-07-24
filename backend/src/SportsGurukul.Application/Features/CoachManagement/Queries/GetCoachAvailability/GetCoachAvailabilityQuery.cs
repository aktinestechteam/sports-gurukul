using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachAvailability;

public class GetCoachAvailabilityQuery : IRequest<Result<AvailabilityDto>>
{
    public Guid CoachId { get; set; }
}
