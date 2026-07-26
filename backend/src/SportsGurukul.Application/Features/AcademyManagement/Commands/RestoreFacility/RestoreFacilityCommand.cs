using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreFacility;

public class RestoreFacilityCommand : IRequest<Result<FacilityDto>>
{
    public Guid FacilityId { get; set; }
}
