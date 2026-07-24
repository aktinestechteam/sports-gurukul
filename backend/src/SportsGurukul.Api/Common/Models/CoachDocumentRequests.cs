using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for uploading a coach document. Uses multipart/form-data encoding.
/// </summary>
public class UploadCoachDocumentRequest
{
    /// <summary>The file to upload.</summary>
    public Microsoft.AspNetCore.Http.IFormFile File { get; set; } = null!;

    /// <summary>Document category.</summary>
    /// <example>CoachingCertification</example>
    public CoachDocumentCategory Category { get; set; }

    /// <summary>Display title for the document.</summary>
    /// <example>Coaching Level 2 Certificate</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description or notes.</summary>
    /// <example>National coaching certification Level 2</example>
    public string? Description { get; set; }

    /// <summary>Optional expiry date for time-sensitive documents.</summary>
    /// <example>2028-01-15</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Optional remarks for the document.</summary>
    /// <example>Issued by National Sports Council</example>
    public string? Remarks { get; set; }

    /// <summary>Whether the document is publicly visible.</summary>
    /// <example>false</example>
    public bool IsPublic { get; set; }
}

/// <summary>
/// Request body for updating coach document metadata. All fields are optional.
/// </summary>
public class UpdateCoachDocumentMetadataRequest
{
    /// <summary>Updated display title.</summary>
    /// <example>Coaching Level 2 Certificate (Updated)</example>
    public string? Title { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Updated national coaching certification</example>
    public string? Description { get; set; }

    /// <summary>Updated document category.</summary>
    /// <example>CoachingCertification</example>
    public CoachDocumentCategory? Category { get; set; }

    /// <summary>Updated expiry date.</summary>
    /// <example>2029-06-30</example>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>Updated remarks.</summary>
    /// <example>Renewed certification</example>
    public string? Remarks { get; set; }

    /// <summary>Updated visibility flag.</summary>
    /// <example>false</example>
    public bool? IsPublic { get; set; }
}

/// <summary>
/// Request body for rejecting a coach document.
/// </summary>
public class RejectCoachDocumentRequest
{
    /// <summary>The reason for rejection. Required.</summary>
    /// <example>Document is blurry and unreadable. Please re-upload a clear copy.</example>
    public string Reason { get; set; } = string.Empty;
}
