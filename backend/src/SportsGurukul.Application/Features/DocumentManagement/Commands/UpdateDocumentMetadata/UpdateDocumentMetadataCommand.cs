using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.UpdateDocumentMetadata;

public class UpdateDocumentMetadataCommand : IRequest<Result<AthleteDocumentDto>>
{
    public Guid DocumentId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DocumentCategory? Category { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool? IsPublic { get; set; }
}
