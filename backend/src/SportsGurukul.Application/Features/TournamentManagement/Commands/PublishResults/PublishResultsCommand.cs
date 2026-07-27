using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.TournamentManagement.Commands.PublishResults;

public class PublishResultsCommand : IRequest<Result<Unit>>
{
    public Guid TournamentId { get; set; }
}
