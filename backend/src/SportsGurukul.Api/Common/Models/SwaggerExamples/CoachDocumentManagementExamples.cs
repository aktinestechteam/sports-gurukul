using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger request example for <see cref="UploadCoachDocumentRequest"/>.
/// </summary>
public class UploadCoachDocumentRequestExample : IExamplesProvider<UploadCoachDocumentRequest>
{
    public UploadCoachDocumentRequest GetExamples() => new()
    {
        Category = CoachDocumentCategory.CoachingCertification,
        Title = "Coaching Level 2 Certificate",
        Description = "National coaching certification Level 2",
        ExpiryDate = DateTime.UtcNow.AddYears(3),
        Remarks = "Issued by National Sports Council",
        IsPublic = false
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateCoachDocumentMetadataRequest"/>.
/// </summary>
public class UpdateCoachDocumentMetadataRequestExample : IExamplesProvider<UpdateCoachDocumentMetadataRequest>
{
    public UpdateCoachDocumentMetadataRequest GetExamples() => new()
    {
        Title = "Coaching Level 2 Certificate (Updated)",
        Description = "Updated national coaching certification",
        Remarks = "Renewed certification",
        IsPublic = false
    };
}

/// <summary>
/// Swagger request example for <see cref="RejectCoachDocumentRequest"/>.
/// </summary>
public class RejectCoachDocumentRequestExample : IExamplesProvider<RejectCoachDocumentRequest>
{
    public RejectCoachDocumentRequest GetExamples() => new()
    {
        Reason = "Document is blurry and unreadable. Please re-upload a clear copy."
    };
}

/// <summary>
/// Swagger response example for <see cref="CoachDocumentDto"/>.
/// </summary>
public class CoachDocumentDtoExample : IExamplesProvider<CoachDocumentDto>
{
    public CoachDocumentDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        CoachId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        Category = "CoachingCertification",
        Title = "Coaching Level 2 Certificate",
        Description = "National coaching certification Level 2",
        OriginalFileName = "coaching_cert_l2.pdf",
        MimeType = "application/pdf",
        Extension = ".pdf",
        FileSize = 524288,
        Checksum = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4",
        Version = 1,
        Status = "Pending",
        UploadedBy = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        UploadedOn = DateTime.UtcNow.AddDays(-1),
        ExpiryDate = DateTime.UtcNow.AddYears(3),
        Remarks = "Issued by National Sports Council",
        IsPublic = false,
        IsDeleted = false,
        DownloadUrl = "/storage/coach-documents/2025/06/a1b2c3d4.pdf",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        UpdatedAt = DateTime.UtcNow.AddDays(-1)
    };
}

/// <summary>
/// Swagger response example for <see cref="CoachDocumentDownloadDto"/>.
/// </summary>
public class CoachDocumentDownloadDtoExample : IExamplesProvider<CoachDocumentDownloadDto>
{
    public CoachDocumentDownloadDto GetExamples() => new()
    {
        DocumentId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        FileName = "coaching_cert_l2.pdf",
        ContentType = "application/pdf",
        FileSize = 524288
    };
}
