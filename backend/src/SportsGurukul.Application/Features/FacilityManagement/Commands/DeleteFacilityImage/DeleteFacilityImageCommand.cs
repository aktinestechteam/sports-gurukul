using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacilityImage;

public class DeleteFacilityImageCommand : IRequest<Result<Unit>>
{
    public Guid ImageId { get; set; }
}
