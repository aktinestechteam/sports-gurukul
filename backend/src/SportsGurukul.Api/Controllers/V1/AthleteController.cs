using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;
using SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateEmergencyContact;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateMedicalProfile;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateRanking;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteAchievements;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteById;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteByUserId;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteProfile;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteRanking;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSports;
using SportsGurukul.Application.Features.AthleteManagement.Queries.SearchAthletes;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages athlete profiles, sports assignments, achievements, medical profiles,
/// emergency contacts, and rankings.
/// </summary>
[ApiController]
[Route("api/v1/athletes")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Athlete Management")]
public class AthleteController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AthleteController> _logger;

    public AthleteController(IMediator mediator, ILogger<AthleteController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Athlete CRUD

    /// <summary>
    /// Creates a new athlete profile linked to an existing user account.
    /// </summary>
    /// <remarks>
    /// Requires <c>Admin</c> role. The athlete profile is linked to an existing user via <c>UserId</c>.
    /// An athlete code is auto-generated (e.g. ATH-20250615-A1B2C3).
    /// </remarks>
    /// <param name="request">Athlete profile details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created athlete profile</returns>
    /// <response code="201">Athlete created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Athlete profile already exists for this user</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(CreateAthleteRequest), typeof(CreateAthleteRequestExample))]
    public async Task<IActionResult> CreateAthlete(
        [FromBody] CreateAthleteRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating athlete profile for user: {UserId}", request.UserId);

        var command = new CreateAthleteCommand
        {
            UserId = request.UserId,
            CurrentLevel = request.CurrentLevel,
            ExperienceYears = request.ExperienceYears,
            Height = request.Height,
            Weight = request.Weight,
            BloodGroup = request.BloodGroup,
            DominantHand = request.DominantHand,
            DominantFoot = request.DominantFoot,
            Biography = request.Biography
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete created: {AthleteId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetAthleteById),
            new { athleteId = result.Value.Id, version = "1.0" },
            ApiResponse<AthleteDto>.SuccessResult(result.Value, "Athlete created successfully."));
    }

    /// <summary>
    /// Searches and lists athlete profiles with filtering, sorting, and pagination.
    /// </summary>
    /// <remarks>
    /// Requires <c>Admin</c> or <c>Coach</c> role. Supports full-text search across name,
    /// athlete code, and sport. Filter by sport, city, state, country, level, ranking,
    /// gender, age range, experience, and status. Sort by name, athletecode, level,
    /// experience, createddate, or updateddate.
    /// </remarks>
    /// <param name="searchTerm">Free-text search across name, athlete code, and sport</param>
    /// <param name="name">Filter by athlete name (partial match)</param>
    /// <param name="sportName">Filter by sport name (partial match)</param>
    /// <param name="city">Filter by city (partial match)</param>
    /// <param name="state">Filter by state (partial match)</param>
    /// <param name="country">Filter by country (partial match)</param>
    /// <param name="currentLevel">Filter by athlete level</param>
    /// <param name="ranking">Filter by ranking (partial match)</param>
    /// <param name="gender">Filter by gender</param>
    /// <param name="minAge">Minimum age filter</param>
    /// <param name="maxAge">Maximum age filter</param>
    /// <param name="minExperience">Minimum years of experience</param>
    /// <param name="maxExperience">Maximum years of experience</param>
    /// <param name="status">Filter by athlete status</param>
    /// <param name="createdFrom">Filter by created date (from)</param>
    /// <param name="createdTo">Filter by created date (to)</param>
    /// <param name="sortBy">Sort field: name, athletecode, level, experience, createddate, or updateddate</param>
    /// <param name="sortDescending">When true, sorts in descending order</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of athlete summaries</returns>
    /// <response code="200">Athletes retrieved successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Coach")]
    [ProducesResponseType(typeof(ApiResponse<AthleteSearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAthletes(
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? name = null,
        [FromQuery] string? sportName = null,
        [FromQuery] string? city = null,
        [FromQuery] string? state = null,
        [FromQuery] string? country = null,
        [FromQuery] Domain.Enums.AthleteLevel? currentLevel = null,
        [FromQuery] string? ranking = null,
        [FromQuery] Domain.Enums.Gender? gender = null,
        [FromQuery] int? minAge = null,
        [FromQuery] int? maxAge = null,
        [FromQuery] int? minExperience = null,
        [FromQuery] int? maxExperience = null,
        [FromQuery] Domain.Enums.AthleteStatus? status = null,
        [FromQuery] DateTime? createdFrom = null,
        [FromQuery] DateTime? createdTo = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Athlete search/list requested: Page={Page}, PageSize={PageSize}", page, pageSize);

        var query = new SearchAthletesQuery
        {
            SearchTerm = searchTerm,
            Name = name,
            SportName = sportName,
            City = city,
            State = state,
            Country = country,
            CurrentLevel = currentLevel,
            Ranking = ranking,
            Gender = gender,
            MinAge = minAge,
            MaxAge = maxAge,
            MinExperience = minExperience,
            MaxExperience = maxExperience,
            Status = status,
            CreatedFrom = createdFrom,
            CreatedTo = createdTo,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AthleteSearchResponse>.SuccessResult(result.Value!, "Athletes retrieved successfully."));
    }

    /// <summary>
    /// Gets a full athlete profile by its unique identifier, including sports,
    /// achievements, medical profile, emergency contact, and ranking.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Full athlete profile</returns>
    /// <response code="200">Athlete profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet("{athleteId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AthleteDtoExample))]
    public async Task<IActionResult> GetAthleteById(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching athlete by ID: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteByIdQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AthleteDto>.SuccessResult(result.Value!, "Athlete retrieved successfully."));
    }

    /// <summary>
    /// Gets a full athlete profile by the associated user identifier.
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Athlete profile linked to this user</returns>
    /// <response code="200">Athlete profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">No athlete profile found for this user</response>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AthleteDtoExample))]
    public async Task<IActionResult> GetAthleteByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching athlete by User ID: {UserId}", userId);

        var result = await _mediator.Send(new GetAthleteByUserIdQuery { UserId = userId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AthleteDto>.SuccessResult(result.Value!, "Athlete retrieved successfully."));
    }

    /// <summary>
    /// Updates an athlete profile. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated athlete profile</returns>
    /// <response code="200">Athlete updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPut("{athleteId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateAthleteRequest), typeof(UpdateAthleteRequestExample))]
    public async Task<IActionResult> UpdateAthlete(
        Guid athleteId,
        [FromBody] UpdateAthleteRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating athlete: {AthleteId}", athleteId);

        var command = new UpdateAthleteCommand
        {
            AthleteId = athleteId,
            CurrentLevel = request.CurrentLevel,
            ExperienceYears = request.ExperienceYears,
            Height = request.Height,
            Weight = request.Weight,
            BloodGroup = request.BloodGroup,
            DominantHand = request.DominantHand,
            DominantFoot = request.DominantFoot,
            Biography = request.Biography,
            Status = request.Status
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete updated: {AthleteId}", athleteId);

        return Ok(ApiResponse<AthleteDto>.SuccessResult(result.Value!, "Athlete updated successfully."));
    }

    /// <summary>
    /// Soft-deletes an athlete profile. Requires <c>Admin</c> role.
    /// </summary>
    /// <remarks>
    /// The athlete is soft-deleted (marked as deleted but not removed from the database).
    /// An Admin can restore the athlete later via the restore endpoint.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Athlete deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Athlete not found</response>
    [HttpDelete("{athleteId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAthlete(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new DeleteAthleteCommand { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete deleted: {AthleteId}", athleteId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted athlete profile. Requires <c>Admin</c> role.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Athlete restored successfully</response>
    /// <response code="400">Athlete is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted athlete found with this ID</response>
    [HttpPost("{athleteId:guid}/restore")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAthlete(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new RestoreAthleteCommand { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Athlete restored: {AthleteId}", athleteId);

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "Athlete restored successfully." },
            "Athlete restored."));
    }

    #endregion

    #region Sports

    /// <summary>
    /// Assigns a sport to an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Sport assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sport assignment</returns>
    /// <response code="201">Sport assigned successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete or sport not found</response>
    /// <response code="409">Sport already assigned to this athlete</response>
    [HttpPost("{athleteId:guid}/sports")]
    [ProducesResponseType(typeof(ApiResponse<SportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(AssignSportRequest), typeof(AssignSportRequestExample))]
    public async Task<IActionResult> AssignSport(
        Guid athleteId,
        [FromBody] AssignSportRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning sport {SportId} to athlete: {AthleteId}", request.SportId, athleteId);

        var command = new AssignSportCommand
        {
            AthleteId = athleteId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Sport assigned to athlete: {AthleteId}", athleteId);

        return CreatedAtAction(
            nameof(GetAthleteSports),
            new { athleteId, version = "1.0" },
            ApiResponse<SportDto>.SuccessResult(result.Value!, "Sport assigned successfully."));
    }

    /// <summary>
    /// Removes a sport assignment from an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="sportId">The sport's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Sport removed successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Sport assignment not found</response>
    [HttpDelete("{athleteId:guid}/sports/{sportId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSport(
        Guid athleteId,
        Guid sportId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing sport {SportId} from athlete: {AthleteId}", sportId, athleteId);

        var result = await _mediator.Send(
            new RemoveSportCommand { AthleteId = athleteId, SportId = sportId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Sport removed from athlete: {AthleteId}", athleteId);

        return NoContent();
    }

    /// <summary>
    /// Gets all sports assigned to an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of assigned sports</returns>
    /// <response code="200">Sports retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet("{athleteId:guid}/sports")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SportDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAthleteSports(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching sports for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteSportsQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<SportDto>>.SuccessResult(result.Value!, "Sports retrieved successfully."));
    }

    #endregion

    #region Achievements

    /// <summary>
    /// Adds a new achievement to an athlete's profile.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Achievement details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created achievement</returns>
    /// <response code="201">Achievement added successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPost("{athleteId:guid}/achievements")]
    [ProducesResponseType(typeof(ApiResponse<AthleteAchievementDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(AddAchievementRequest), typeof(AddAchievementRequestExample))]
    public async Task<IActionResult> AddAchievement(
        Guid athleteId,
        [FromBody] AddAchievementRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding achievement to athlete: {AthleteId}", athleteId);

        var command = new AddAchievementCommand
        {
            AthleteId = athleteId,
            Title = request.Title,
            Competition = request.Competition,
            Position = request.Position,
            Level = request.Level,
            Date = request.Date,
            CertificateUrl = request.CertificateUrl,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Achievement added to athlete: {AthleteId}", athleteId);

        return CreatedAtAction(
            nameof(GetAthleteAchievements),
            new { athleteId, version = "1.0" },
            ApiResponse<AthleteAchievementDto>.SuccessResult(result.Value!, "Achievement added successfully."));
    }

    /// <summary>
    /// Updates an existing achievement. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated achievement</returns>
    /// <response code="200">Achievement updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Achievement not found</response>
    [HttpPut("{athleteId:guid}/achievements/{achievementId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AthleteAchievementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateAchievementRequest), typeof(UpdateAchievementRequestExample))]
    public async Task<IActionResult> UpdateAchievement(
        Guid athleteId,
        Guid achievementId,
        [FromBody] UpdateAchievementRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating achievement {AchievementId} for athlete: {AthleteId}", achievementId, athleteId);

        var command = new UpdateAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId,
            Title = request.Title,
            Competition = request.Competition,
            Position = request.Position,
            Level = request.Level,
            Date = request.Date,
            CertificateUrl = request.CertificateUrl,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Achievement updated: {AchievementId}", achievementId);

        return Ok(ApiResponse<AthleteAchievementDto>.SuccessResult(result.Value!, "Achievement updated successfully."));
    }

    /// <summary>
    /// Removes an achievement from an athlete's profile.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="achievementId">The achievement's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Achievement removed successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Achievement not found</response>
    [HttpDelete("{athleteId:guid}/achievements/{achievementId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAchievement(
        Guid athleteId,
        Guid achievementId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting achievement {AchievementId} from athlete: {AthleteId}", achievementId, athleteId);

        var result = await _mediator.Send(
            new DeleteAchievementCommand { AthleteId = athleteId, AchievementId = achievementId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Achievement deleted: {AchievementId}", achievementId);

        return NoContent();
    }

    /// <summary>
    /// Gets all achievements for an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of achievements</returns>
    /// <response code="200">Achievements retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet("{athleteId:guid}/achievements")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AthleteAchievementDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAthleteAchievements(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching achievements for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteAchievementsQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AthleteAchievementDto>>.SuccessResult(result.Value!, "Achievements retrieved successfully."));
    }

    #endregion

    #region Medical Profile

    /// <summary>
    /// Gets the medical profile for an athlete.
    /// </summary>
    /// <remarks>
    /// Medical information is sensitive and is never logged.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Medical profile</returns>
    /// <response code="200">Medical profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete or medical profile not found</response>
    [HttpGet("{athleteId:guid}/medical-profile")]
    [ProducesResponseType(typeof(ApiResponse<MedicalProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMedicalProfile(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching medical profile for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteProfileQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        if (result.Value!.MedicalProfile is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "Medical profile not found for this athlete.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        return Ok(ApiResponse<MedicalProfileDto>.SuccessResult(result.Value.MedicalProfile, "Medical profile retrieved successfully."));
    }

    /// <summary>
    /// Updates the medical profile for an athlete. All fields are optional.
    /// Creates the profile if it does not already exist.
    /// </summary>
    /// <remarks>
    /// Medical information is never logged for privacy compliance.
    /// </remarks>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Medical profile fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated medical profile</returns>
    /// <response code="200">Medical profile updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPut("{athleteId:guid}/medical-profile")]
    [ProducesResponseType(typeof(ApiResponse<MedicalProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateMedicalProfileRequest), typeof(UpdateMedicalProfileRequestExample))]
    public async Task<IActionResult> UpdateMedicalProfile(
        Guid athleteId,
        [FromBody] UpdateMedicalProfileRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating medical profile for athlete: {AthleteId}", athleteId);

        var command = new UpdateMedicalProfileCommand
        {
            AthleteId = athleteId,
            MedicalConditions = request.MedicalConditions,
            Allergies = request.Allergies,
            Medications = request.Medications,
            BloodGroup = request.BloodGroup,
            InsuranceNumber = request.InsuranceNumber,
            DoctorName = request.DoctorName,
            DoctorContact = request.DoctorContact
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Medical profile updated for athlete: {AthleteId}", athleteId);

        return Ok(ApiResponse<MedicalProfileDto>.SuccessResult(result.Value!, "Medical profile updated successfully."));
    }

    #endregion

    #region Emergency Contact

    /// <summary>
    /// Gets the emergency contact for an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Emergency contact</returns>
    /// <response code="200">Emergency contact retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete or emergency contact not found</response>
    [HttpGet("{athleteId:guid}/emergency-contact")]
    [ProducesResponseType(typeof(ApiResponse<EmergencyContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmergencyContact(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching emergency contact for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteProfileQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        if (result.Value!.EmergencyContact is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = "Emergency contact not found for this athlete.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });
        }

        return Ok(ApiResponse<EmergencyContactDto>.SuccessResult(result.Value.EmergencyContact, "Emergency contact retrieved successfully."));
    }

    /// <summary>
    /// Updates the emergency contact for an athlete. Creates the contact if it does not exist.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Emergency contact details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated emergency contact</returns>
    /// <response code="200">Emergency contact updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPut("{athleteId:guid}/emergency-contact")]
    [ProducesResponseType(typeof(ApiResponse<EmergencyContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateEmergencyContactRequest), typeof(UpdateEmergencyContactRequestExample))]
    public async Task<IActionResult> UpdateEmergencyContact(
        Guid athleteId,
        [FromBody] UpdateEmergencyContactRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating emergency contact for athlete: {AthleteId}", athleteId);

        var command = new UpdateEmergencyContactCommand
        {
            AthleteId = athleteId,
            Name = request.Name,
            Relationship = request.Relationship,
            Phone = request.Phone,
            Email = request.Email
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Emergency contact updated for athlete: {AthleteId}", athleteId);

        return Ok(ApiResponse<EmergencyContactDto>.SuccessResult(result.Value!, "Emergency contact updated successfully."));
    }

    #endregion

    #region Ranking

    /// <summary>
    /// Gets the ranking information for an athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Ranking details</returns>
    /// <response code="200">Ranking retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete or ranking not found</response>
    [HttpGet("{athleteId:guid}/ranking")]
    [ProducesResponseType(typeof(ApiResponse<RankingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RankingDtoExample))]
    public async Task<IActionResult> GetRanking(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching ranking for athlete: {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetAthleteRankingQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<RankingDto>.SuccessResult(result.Value!, "Ranking retrieved successfully."));
    }

    /// <summary>
    /// Updates the ranking information for an athlete. Creates the ranking if it does not exist.
    /// All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="request">Ranking fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated ranking</returns>
    /// <response code="200">Ranking updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Athlete not found</response>
    [HttpPut("{athleteId:guid}/ranking")]
    [ProducesResponseType(typeof(ApiResponse<RankingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateRankingRequest), typeof(UpdateRankingRequestExample))]
    public async Task<IActionResult> UpdateRanking(
        Guid athleteId,
        [FromBody] UpdateRankingRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating ranking for athlete: {AthleteId}", athleteId);

        var command = new UpdateRankingCommand
        {
            AthleteId = athleteId,
            CurrentRank = request.CurrentRank,
            StateRank = request.StateRank,
            NationalRank = request.NationalRank,
            InternationalRank = request.InternationalRank,
            RankingAuthority = request.RankingAuthority
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Ranking updated for athlete: {AthleteId}", athleteId);

        return Ok(ApiResponse<RankingDto>.SuccessResult(result.Value!, "Ranking updated successfully."));
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

        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already associated", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
            });
        }

        if (error.Contains("deleted", StringComparison.OrdinalIgnoreCase) &&
            error.Contains("restore", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Bad Request",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
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
