using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacility;

public class DeleteFacilityCommand : IRequest<Result<Unit>>
{
    public Guid FacilityId { get; set; }
}
