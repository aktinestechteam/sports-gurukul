using MediatR;
using Microsoft.AspNetCore.Http;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UploadCoachDocument;

public class UploadCoachDocumentCommand : IRequest<Result<CoachDocumentDto>>
{
    public Guid CoachId { get; set; }
    public IFormFile File { get; set; } = null!;
    public CoachDocumentCategory Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Remarks { get; set; }
    public bool IsPublic { get; set; }
}
