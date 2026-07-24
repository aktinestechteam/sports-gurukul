using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.DownloadCoachDocument;

public class DownloadCoachDocumentQuery : IRequest<Result<CoachDocumentDownloadDto>>
{
    public Guid DocumentId { get; set; }
}
