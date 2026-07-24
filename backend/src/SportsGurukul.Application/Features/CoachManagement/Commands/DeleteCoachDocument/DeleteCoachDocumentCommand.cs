using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoachDocument;

public class DeleteCoachDocumentCommand : IRequest<Result<Unit>>
{
    public Guid DocumentId { get; set; }
}
