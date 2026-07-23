using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.RestoreAthleteDocument;

public class RestoreAthleteDocumentCommand : IRequest<Result<AthleteDocumentDto>>
{
    public Guid DocumentId { get; set; }
}
