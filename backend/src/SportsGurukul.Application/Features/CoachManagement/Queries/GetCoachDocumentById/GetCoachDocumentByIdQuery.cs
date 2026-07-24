using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocumentById;

public class GetCoachDocumentByIdQuery : IRequest<Result<CoachDocumentDto>>
{
    public Guid DocumentId { get; set; }
}
