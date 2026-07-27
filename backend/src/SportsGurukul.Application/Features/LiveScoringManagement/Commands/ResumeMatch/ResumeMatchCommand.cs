using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.LiveScoringManagement.Commands.ResumeMatch;

public class ResumeMatchCommand : IRequest<Result<Unit>>
{
    public Guid MatchId { get; set; }
}
