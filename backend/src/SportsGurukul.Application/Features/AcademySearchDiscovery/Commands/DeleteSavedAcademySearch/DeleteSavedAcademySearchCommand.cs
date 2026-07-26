using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.DeleteSavedAcademySearch;

public class DeleteSavedAcademySearchCommand : IRequest<Result<Unit>>
{
    public Guid SearchId { get; set; }
    public Guid UserId { get; set; }
}
