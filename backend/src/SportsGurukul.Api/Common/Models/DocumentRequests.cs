using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for uploading a document. Uses multipart/form-data encoding.
/// </summary>
public class UploadDocumentRequest
{
    /// <summary>The file to upload.</summary>
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;

    /// <summary>Document category.</summary>
    /// <example>IdProof</example>
    public DocumentCategory Category { get; set; }

    /// <summary>Display title for the document.</summary>
    /// <example>Aadhaar Card - Front</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description or notes.</summary>
    /// <example>Front side of Aadhaar card</example>
    public string? Description { get; set; }

    /// <summary>Optional expiry date for time-sensitive documents.</summary>
    /// <example>2028-01-15</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Whether the document is publicly visible.</summary>
    /// <example>false</example>
    public bool IsPublic { get; set; }
}

/// <summary>
/// Request body for updating document metadata. All fields are optional.
/// </summary>
public class UpdateDocumentMetadataRequest
{
    /// <summary>Updated display title.</summary>
    /// <example>Aadhaar Card - Front (Updated)</example>
    public string? Title { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Updated scan of Aadhaar card</example>
    public string? Description { get; set; }

    /// <summary>Updated document category.</summary>
    /// <example>IdProof</example>
    public DocumentCategory? Category { get; set; }

    /// <summary>Updated expiry date.</summary>
    /// <example>2029-06-30</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Updated visibility flag.</summary>
    /// <example>false</example>
    public bool? IsPublic { get; set; }
}
