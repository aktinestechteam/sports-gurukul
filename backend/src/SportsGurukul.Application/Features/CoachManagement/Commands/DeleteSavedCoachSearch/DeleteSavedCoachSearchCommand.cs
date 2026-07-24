using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteSavedCoachSearch;

public class DeleteSavedCoachSearchCommand : IRequest<Result<Unit>>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
