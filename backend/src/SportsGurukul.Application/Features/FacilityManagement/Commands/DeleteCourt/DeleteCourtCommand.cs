using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteCourt;

public class DeleteCourtCommand : IRequest<Result<Unit>>
{
    public Guid CourtId { get; set; }
}
