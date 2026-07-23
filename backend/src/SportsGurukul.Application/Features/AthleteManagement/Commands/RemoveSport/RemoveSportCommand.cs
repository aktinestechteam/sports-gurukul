using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;

public class RemoveSportCommand : IRequest<Result<Unit>>
{
    public Guid AthleteId { get; set; }
    public Guid SportId { get; set; }
}
