using MediatR;
using Microsoft.AspNetCore.Http;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;

public class UploadAthleteDocumentCommand : IRequest<Result<AthleteDocumentDto>>
{
    public Guid AthleteId { get; set; }
    public IFormFile File { get; set; } = null!;
    public DocumentCategory Category { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsPublic { get; set; }
}
