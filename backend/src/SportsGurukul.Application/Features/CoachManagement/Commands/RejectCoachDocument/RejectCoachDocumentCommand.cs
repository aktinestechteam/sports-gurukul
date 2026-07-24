using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RejectCoachDocument;

public class RejectCoachDocumentCommand : IRequest<Result<CoachDocumentDto>>
{
    public Guid DocumentId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
