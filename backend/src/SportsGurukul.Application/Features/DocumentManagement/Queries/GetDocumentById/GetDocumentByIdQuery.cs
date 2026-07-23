using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.GetDocumentById;

public class GetDocumentByIdQuery : IRequest<Result<AthleteDocumentDto>>
{
    public Guid DocumentId { get; set; }
}
