using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.DeleteAthleteDocument;

public class DeleteAthleteDocumentCommand : IRequest<Result<Unit>>
{
    public Guid DocumentId { get; set; }
}
