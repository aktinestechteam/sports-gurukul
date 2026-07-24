using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocuments;

public class GetCoachDocumentsQuery : IRequest<Result<IReadOnlyList<CoachDocumentDto>>>
{
    public Guid CoachId { get; set; }
}
