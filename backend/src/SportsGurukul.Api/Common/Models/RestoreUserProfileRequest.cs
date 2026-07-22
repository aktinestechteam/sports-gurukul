namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for restoring a soft-deleted user profile.
/// Requires <c>Admin</c> or <c>SuperAdmin</c> role.
/// </summary>
public class RestoreUserProfileRequest
{
    /// <summary>Unique identifier of the user whose profile should be restored.</summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid UserId { get; set; }
}
