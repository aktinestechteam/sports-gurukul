using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.RestoreFacility;

public class RestoreFacilityCommand : IRequest<Result<FacilityDetailDto>>
{
    public Guid FacilityId { get; set; }
}
