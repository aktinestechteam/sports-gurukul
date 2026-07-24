using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;

public class RemoveSportCommand : IRequest<Result<Unit>>
{
    public Guid CoachId { get; set; }
    public Guid SportId { get; set; }
}
