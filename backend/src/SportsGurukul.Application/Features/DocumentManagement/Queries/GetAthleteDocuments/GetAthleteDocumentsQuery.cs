using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.GetAthleteDocuments;

public class GetAthleteDocumentsQuery : IRequest<Result<IReadOnlyList<AthleteDocumentDto>>>
{
    public Guid AthleteId { get; set; }
}
