using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.DownloadDocument;

public class DownloadDocumentQuery : IRequest<Result<DocumentDownloadDto>>
{
    public Guid DocumentId { get; set; }
}
