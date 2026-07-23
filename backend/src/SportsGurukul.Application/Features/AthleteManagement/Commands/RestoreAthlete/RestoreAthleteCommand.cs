using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;

public class RestoreAthleteCommand : IRequest<Result<Unit>>
{
    public Guid AthleteId { get; set; }
}
