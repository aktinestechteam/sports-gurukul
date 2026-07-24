using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoachDocument;

public class RestoreCoachDocumentCommand : IRequest<Result<CoachDocumentDto>>
{
    public Guid DocumentId { get; set; }
}
