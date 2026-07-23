using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Application.Features.UserManagement.Commands.DeleteProfilePhoto;
using SportsGurukul.Application.Features.UserManagement.Commands.DeleteUserProfile;
using SportsGurukul.Application.Features.UserManagement.Commands.RestoreUserProfile;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserProfile;
using SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;
using SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Application.Features.UserManagement.Queries.GetCurrentUser;
using SportsGurukul.Application.Features.UserManagement.Queries.GetProfilePhoto;
using SportsGurukul.Application.Features.UserManagement.Queries.GetUserById;
using SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages user profiles: retrieval, update, soft-delete, restore, search, and preferences.
/// </summary>
[ApiController]
[Route("api/v1/users")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("User Management")]
public class UserProfileController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserProfileController> _logger;

    public UserProfileController(IMediator mediator, ILogger<UserProfileController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Current User Profile

    /// <summary>
    /// Gets the profile of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The current user's full profile</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Profile not found for this user</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Profile retrieval requested for current user");

        var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<UserProfileDto>.SuccessResult(result.Value!, "Profile retrieved successfully."));
    }

    /// <summary>
    /// Gets a user profile by its unique identifier. Requires Admin or SuperAdmin role.
    /// </summary>
    /// <param name="id">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The requested user profile</returns>
    /// <response code="200">Profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">User not found</response>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Profile retrieval requested for user: {UserId}", id);

        var result = await _mediator.Send(new GetUserByIdQuery { UserId = id }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<UserProfileDto>.SuccessResult(result.Value!, "Profile retrieved successfully."));
    }

    /// <summary>
    /// Updates the profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Only the profile owner can update their own profile. All fields are optional —
    /// only provided fields will be updated. An address is created or updated when
    /// <c>AddressLine1</c> and <c>City</c> are supplied.
    /// </remarks>
    /// <param name="request">Profile fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated profile</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Profile not found — create a profile first</response>
    [HttpPut("me")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateUserProfileRequest), typeof(UpdateUserProfileRequestExample))]
    public async Task<IActionResult> UpdateCurrentUser(
        [FromBody] UpdateUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Profile update requested for current user");

        var command = new UpdateUserProfileCommand
        {
            UserId = userId.Value,
            DateOfBirth = request.DateOfBirth,
            Gender = request.Gender,
            Bio = request.Bio,
            Height = request.Height,
            Weight = request.Weight,
            PreferredSport = request.PreferredSport,
            ExperienceLevel = request.ExperienceLevel,
            PrimaryPhoneCountryCode = request.PrimaryPhoneCountryCode,
            PrimaryPhoneNumber = request.PrimaryPhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            AddressType = request.AddressType
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Profile updated for current user");

        return Ok(ApiResponse<UserProfileDto>.SuccessResult(result.Value!, "Profile updated successfully."));
    }

    /// <summary>
    /// Soft-deletes the profile of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// The profile is soft-deleted (marked as deleted but not removed from the database).
    /// An Admin can restore the profile later via the restore endpoint.
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Profile deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Profile deletion requested for current user");

        var command = new DeleteUserProfileCommand { UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Profile deleted for current user");

        return NoContent();
    }

    #endregion

    #region Restore

    /// <summary>
    /// Restores a previously soft-deleted user profile. Requires Admin or SuperAdmin role.
    /// </summary>
    /// <param name="request">The user identifier of the profile to restore</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Profile restored successfully</response>
    /// <response code="400">Validation error or profile not restorable</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted profile found for this user</response>
    [HttpPost("restore")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(RestoreUserProfileRequest), typeof(RestoreUserProfileRequestExample))]
    public async Task<IActionResult> RestoreUser(
        [FromBody] RestoreUserProfileRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Profile restore requested for user: {TargetUserId}", request.UserId);

        var command = new RestoreUserProfileCommand { UserId = request.UserId };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Profile restored for user: {TargetUserId}", request.UserId);

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "Profile restored successfully." },
            "Profile restored."));
    }

    #endregion

    #region List & Search

    /// <summary>
    /// Searches and lists user profiles with filtering, sorting, and pagination.
    /// Requires Admin or SuperAdmin role.
    /// </summary>
    /// <remarks>
    /// Supports full-text search across name, email, mobile, sport, and bio.
    /// Filter by role, status, gender, country, state, city, email verified, active, and deleted.
    /// Sort by name, email, role, createddate, or updateddate (ascending or descending).
    /// </remarks>
    /// <param name="searchTerm">Free-text search across name, email, mobile, sport, and bio</param>
    /// <param name="name">Filter by name (partial match)</param>
    /// <param name="email">Filter by email (partial match)</param>
    /// <param name="mobile">Filter by mobile number (partial match)</param>
    /// <param name="city">Filter by city (partial match)</param>
    /// <param name="state">Filter by state (partial match)</param>
    /// <param name="country">Filter by country (partial match)</param>
    /// <param name="role">Filter by role type</param>
    /// <param name="status">Filter by user status</param>
    /// <param name="gender">Filter by gender</param>
    /// <param name="emailVerified">Filter by email verification status</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="isDeleted">Filter by deleted status</param>
    /// <param name="createdFrom">Filter by created date (from)</param>
    /// <param name="createdTo">Filter by created date (to)</param>
    /// <param name="updatedFrom">Filter by updated date (from)</param>
    /// <param name="updatedTo">Filter by updated date (to)</param>
    /// <param name="sortBy">Sort field: name, email, role, createddate, or updateddate</param>
    /// <param name="sortDescending">When true, sorts in descending order</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of user summaries</returns>
    /// <response code="200">Users retrieved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    [ProducesResponseType(typeof(ApiResponse<UserSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? name = null,
        [FromQuery] string? email = null,
        [FromQuery] string? mobile = null,
        [FromQuery] string? city = null,
        [FromQuery] string? state = null,
        [FromQuery] string? country = null,
        [FromQuery] RoleType? role = null,
        [FromQuery] UserStatus? status = null,
        [FromQuery] Gender? gender = null,
        [FromQuery] bool? emailVerified = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isDeleted = null,
        [FromQuery] DateTime? createdFrom = null,
        [FromQuery] DateTime? createdTo = null,
        [FromQuery] DateTime? updatedFrom = null,
        [FromQuery] DateTime? updatedTo = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User search/list requested: Page={Page}, PageSize={PageSize}", page, pageSize);

        var query = new SearchUsersQuery
        {
            SearchTerm = searchTerm,
            Name = name,
            Email = email,
            Mobile = mobile,
            City = city,
            State = state,
            Country = country,
            Role = role,
            Status = status,
            Gender = gender,
            EmailVerified = emailVerified,
            IsActive = isActive,
            IsDeleted = isDeleted,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo,
            UpdatedFrom = updatedFrom,
            UpdatedTo = updatedTo,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<UserSearchResponse>.SuccessResult(result.Value!, "Users retrieved successfully."));
    }

    #endregion

    #region Preferences

    /// <summary>
    /// Updates the preferences of the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// All fields are optional — only provided fields will be updated.
    /// Preferences are created automatically if they don't already exist for this user.
    /// </remarks>
    /// <param name="request">Preference fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated preferences</returns>
    /// <response code="200">Preferences updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Profile not found — create a profile first</response>
    [HttpPut("preferences")]
    [ProducesResponseType(typeof(ApiResponse<UserPreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateUserPreferenceRequest), typeof(UpdateUserPreferenceRequestExample))]
    public async Task<IActionResult> UpdatePreferences(
        [FromBody] UpdateUserPreferenceRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Preference update requested for current user");

        var command = new UpdateUserPreferenceCommand
        {
            UserId = userId.Value,
            Language = request.Language,
            Theme = request.Theme,
            TimeZone = request.TimeZone,
            EmailNotifications = request.EmailNotifications,
            PushNotifications = request.PushNotifications,
            SmsNotifications = request.SmsNotifications,
            MarketingEmails = request.MarketingEmails,
            ProfileVisibility = request.ProfileVisibility,
            ShowOnlineStatus = request.ShowOnlineStatus
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Preferences updated for current user");

        return Ok(ApiResponse<UserPreferenceDto>.SuccessResult(result.Value!, "Preferences updated successfully."));
    }

    #endregion

    #region Profile Photo

    /// <summary>
    /// Uploads a profile photo for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Accepts JPEG, PNG, or WebP images up to 5 MB. Replaces any existing profile photo.
    /// The file is stored using the configured storage provider (local, Azure Blob, or S3).
    /// </remarks>
    /// <param name="file">The image file to upload</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Uploaded file metadata</returns>
    /// <response code="200">Photo uploaded successfully</response>
    /// <response code="400">Invalid file type or size</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Profile not found</response>
    [HttpPost("me/photo")]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(ApiResponse<ProfilePhotoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UploadProfilePhoto(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Profile photo upload requested for current user");

        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            fileContent = memoryStream.ToArray();
        }

        var command = new UploadProfilePhotoCommand
        {
            UserId = userId.Value,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileContent = fileContent
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Profile photo uploaded for current user");

        return Ok(ApiResponse<ProfilePhotoResponse>.SuccessResult(result.Value!, "Profile photo uploaded successfully."));
    }

    /// <summary>
    /// Gets the profile photo metadata for the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Profile photo metadata including URL</returns>
    /// <response code="200">Photo metadata retrieved</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">No profile photo found</response>
    [HttpGet("me/photo")]
    [ProducesResponseType(typeof(ApiResponse<ProfilePhotoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfilePhoto(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Profile photo retrieval requested for current user");

        var query = new GetProfilePhotoQuery { UserId = userId.Value };
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ProfilePhotoResponse>.SuccessResult(result.Value!, "Profile photo retrieved successfully."));
    }

    /// <summary>
    /// Deletes the profile photo of the currently authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Photo deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">No profile photo found</response>
    [HttpDelete("me/photo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfilePhoto(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId is null)
            return Unauthorized(new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = "Invalid user identity.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            });

        _logger.LogInformation("Profile photo deletion requested for current user");

        var command = new DeleteProfilePhotoCommand { UserId = userId.Value };
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Profile photo deleted for current user");

        return NoContent();
    }

    #endregion

    #region Helpers

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });
        }

        return BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = error,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }

    #endregion
}
