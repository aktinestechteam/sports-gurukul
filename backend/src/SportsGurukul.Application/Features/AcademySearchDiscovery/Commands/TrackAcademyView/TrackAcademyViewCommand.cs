using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.TrackAcademyView;

public class TrackAcademyViewCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
    public Guid? UserId { get; set; }
    public string Source { get; set; } = string.Empty;
}
