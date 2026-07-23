using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

/// <summary>
/// Swagger request example for <see cref="UploadDocumentRequest"/>.
/// </summary>
public class UploadDocumentRequestExample : IExamplesProvider<UploadDocumentRequest>
{
    public UploadDocumentRequest GetExamples() => new()
    {
        Category = DocumentCategory.IdentityDocument,
        Title = "Aadhaar Card - Front",
        Description = "Front side of Aadhaar card for identity verification",
        ExpiryDate = DateTime.UtcNow.AddYears(3),
        IsPublic = false
    };
}

/// <summary>
/// Swagger request example for <see cref="UpdateDocumentMetadataRequest"/>.
/// </summary>
public class UpdateDocumentMetadataRequestExample : IExamplesProvider<UpdateDocumentMetadataRequest>
{
    public UpdateDocumentMetadataRequest GetExamples() => new()
    {
        Title = "Aadhaar Card - Front (Updated)",
        Description = "Updated scan with better resolution",
        IsPublic = false
    };
}

/// <summary>
/// Swagger response example for <see cref="AthleteDocumentDto"/>.
/// </summary>
public class AthleteDocumentDtoExample : IExamplesProvider<AthleteDocumentDto>
{
    public AthleteDocumentDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        AthleteId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        Category = "IdProof",
        Title = "Aadhaar Card - Front",
        Description = "Front side of Aadhaar card for identity verification",
        OriginalFileName = "aadhaar_front.jpg",
        MimeType = "image/jpeg",
        Extension = ".jpg",
        FileSize = 245760,
        Checksum = "a1b2c3d4e5f6a1b2c3d4e5f6a1b2c3d4",
        Version = 1,
        Status = "Pending",
        UploadedBy = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        UploadedOn = DateTime.UtcNow.AddDays(-1),
        ExpiryDate = DateTime.UtcNow.AddYears(3),
        IsPublic = false,
        DownloadUrl = "/storage/documents/2025/06/a1b2c3d4.jpg",
        CreatedAt = DateTime.UtcNow.AddDays(-1),
        UpdatedAt = DateTime.UtcNow.AddDays(-1)
    };
}

/// <summary>
/// Swagger response example for <see cref="DocumentAuditDto"/>.
/// </summary>
public class DocumentAuditDtoExample : IExamplesProvider<DocumentAuditDto>
{
    public DocumentAuditDto GetExamples() => new()
    {
        Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        DocumentId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        Action = "Uploaded",
        PerformedBy = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        PerformedOn = DateTime.UtcNow.AddDays(-1),
        Details = "Document uploaded: aadhaar_front.jpg"
    };
}
