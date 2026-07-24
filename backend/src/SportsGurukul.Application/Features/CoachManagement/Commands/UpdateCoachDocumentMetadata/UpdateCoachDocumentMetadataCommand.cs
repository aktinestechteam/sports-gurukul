using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachDocumentMetadata;

public class UpdateCoachDocumentMetadataCommand : IRequest<Result<CoachDocumentDto>>
{
    public Guid DocumentId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public CoachDocumentCategory? Category { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public bool? IsPublic { get; set; }
}
