using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class ProfilePhotoResponse
{
    public Guid FileId { get; set; }
    public string Url { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
}
