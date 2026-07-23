using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;

public class DeleteAthleteCommand : IRequest<Result<Unit>>
{
    public Guid AthleteId { get; set; }
}
