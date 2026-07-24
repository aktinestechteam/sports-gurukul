using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCoachDocument;

public class VerifyCoachDocumentCommand : IRequest<Result<CoachDocumentDto>>
{
    public Guid DocumentId { get; set; }
    public string? Comments { get; set; }
}
