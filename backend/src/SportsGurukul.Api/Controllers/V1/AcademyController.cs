using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.AcademyManagement.Commands.CreateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.DeleteAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RejectAcademyVerification;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RestoreAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateAcademy;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateContact;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateOperatingHours;
using SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateSocialLinks;
using SportsGurukul.Application.Features.AcademyManagement.Commands.VerifyAcademy;
using SocialLinkInput = SportsGurukul.Application.Features.AcademyManagement.Commands.UpdateSocialLinks.SocialLinkInput;
using SportsGurukul.Application.Features.AcademyManagement.Commands.AssignSport;
using SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveSport;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyById;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetAcademyProfile;
using SportsGurukul.Application.Features.AcademyManagement.Queries.GetOperatingHours;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages academy profiles, contact information, operating hours, social links, sports assignments,
/// and verification workflows.
/// </summary>
[ApiController]
[Route("api/v1/academies")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Academy Management")]
public class AcademyController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AcademyController> _logger;

    public AcademyController(IMediator mediator, ILogger<AcademyController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Academy CRUD

    /// <summary>
    /// Creates a new academy profile. An academy code is auto-generated (e.g. ACAD-20250615-A1B2C3).
    /// </summary>
    /// <param name="request">Academy profile details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created academy profile</returns>
    /// <response code="201">Academy created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="409">Academy with the same email or registration number already exists</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(CreateAcademyRequest), typeof(CreateAcademyRequestExample))]
    public async Task<IActionResult> CreateAcademy(
        [FromBody] CreateAcademyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating academy: {AcademyName}", request.Name);

        var command = new CreateAcademyCommand
        {
            Name = request.Name,
            LegalName = request.LegalName,
            Description = request.Description,
            RegistrationNumber = request.RegistrationNumber,
            GSTNumber = request.GSTNumber,
            EstablishedDate = request.EstablishedDate,
            Website = request.Website,
            Email = request.Email,
            Phone = request.Phone
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy created: {AcademyId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetAcademyById),
            new { academyId = result.Value.Id, version = "1.0" },
            ApiResponse<AcademyDto>.SuccessResult(result.Value, "Academy created successfully."));
    }

    /// <summary>
    /// Gets a full academy profile by its unique identifier, including branches, facilities,
    /// sports, memberships, contact, operating hours, and social links.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Full academy profile</returns>
    /// <response code="200">Academy profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet("{academyId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademyDtoExample))]
    public async Task<IActionResult> GetAcademyById(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching academy by ID: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetAcademyByIdQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AcademyDto>.SuccessResult(result.Value!, "Academy retrieved successfully."));
    }

    /// <summary>
    /// Gets a full academy profile by its unique code (e.g. ACAD-20250615-A1B2C3).
    /// </summary>
    /// <param name="academyCode">The academy's unique code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Full academy profile</returns>
    /// <response code="200">Academy profile retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet("code/{academyCode}")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(AcademyDtoExample))]
    public async Task<IActionResult> GetAcademyByCode(
        string academyCode,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching academy by code: {AcademyCode}", academyCode);

        var result = await _mediator.Send(new GetAcademyProfileQuery { AcademyCode = academyCode }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AcademyDto>.SuccessResult(result.Value!, "Academy retrieved successfully."));
    }

    /// <summary>
    /// Updates an academy profile. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated academy profile</returns>
    /// <response code="200">Academy updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPut("{academyId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateAcademyRequest), typeof(UpdateAcademyRequestExample))]
    public async Task<IActionResult> UpdateAcademy(
        Guid academyId,
        [FromBody] UpdateAcademyRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating academy: {AcademyId}", academyId);

        var command = new UpdateAcademyCommand
        {
            AcademyId = academyId,
            Name = request.Name,
            LegalName = request.LegalName,
            Description = request.Description,
            RegistrationNumber = request.RegistrationNumber,
            GSTNumber = request.GSTNumber,
            EstablishedDate = request.EstablishedDate,
            Website = request.Website,
            Email = request.Email,
            Phone = request.Phone,
            LogoUrl = request.LogoUrl,
            BannerUrl = request.BannerUrl
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy updated: {AcademyId}", academyId);

        return Ok(ApiResponse<AcademyDto>.SuccessResult(result.Value!, "Academy updated successfully."));
    }

    /// <summary>
    /// Soft-deletes an academy. Requires System Admin role.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Academy deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpDelete("{academyId:guid}")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAcademy(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new DeleteAcademyCommand { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy deleted: {AcademyId}", academyId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted academy. Requires System Admin role.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Academy restored successfully</response>
    /// <response code="400">Academy is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted academy found with this ID</response>
    [HttpPost("{academyId:guid}/restore")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<MessageResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreAcademy(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new RestoreAcademyCommand { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy restored: {AcademyId}", academyId);

        return Ok(ApiResponse<MessageResponse>.SuccessResult(
            new MessageResponse { Message = "Academy restored successfully." },
            "Academy restored."));
    }

    #endregion

    #region Verification

    /// <summary>
    /// Verifies an academy. Requires System Admin role.
    /// Sets the academy's verification status to Verified.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Verification details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated academy profile</returns>
    /// <response code="200">Academy verified successfully</response>
    /// <response code="400">Academy is not in a verifiable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPost("{academyId:guid}/verify")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(VerifyAcademyRequest), typeof(VerifyAcademyRequestExample))]
    public async Task<IActionResult> VerifyAcademy(
        Guid academyId,
        [FromBody] VerifyAcademyRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verifying academy: {AcademyId}", academyId);

        var command = new VerifyAcademyCommand
        {
            AcademyId = academyId,
            Remarks = request?.Remarks
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy verified: {AcademyId}", academyId);

        return Ok(ApiResponse<AcademyDto>.SuccessResult(result.Value!, "Academy verified successfully."));
    }

    /// <summary>
    /// Rejects an academy verification request. Requires System Admin role.
    /// Sets the academy's verification status to Rejected with a reason.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Rejection details with reason</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated academy profile</returns>
    /// <response code="200">Academy verification rejected</response>
    /// <response code="400">Academy is not in a verifiable state</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPost("{academyId:guid}/reject")]
    [Authorize(Roles = "System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(RejectAcademyVerificationRequest), typeof(RejectAcademyVerificationRequestExample))]
    public async Task<IActionResult> RejectAcademyVerification(
        Guid academyId,
        [FromBody] RejectAcademyVerificationRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting academy verification: {AcademyId}", academyId);

        var command = new RejectAcademyVerificationCommand
        {
            AcademyId = academyId,
            Remarks = request.Remarks
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Academy verification rejected: {AcademyId}", academyId);

        return Ok(ApiResponse<AcademyDto>.SuccessResult(result.Value!, "Academy verification rejected."));
    }

    #endregion

    #region Contact & Operating Hours

    /// <summary>
    /// Updates the contact information for an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Contact fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated contact details</returns>
    /// <response code="200">Contact updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPut("{academyId:guid}/contact")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<ContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateAcademyContactRequest), typeof(UpdateAcademyContactRequestExample))]
    public async Task<IActionResult> UpdateContact(
        Guid academyId,
        [FromBody] UpdateAcademyContactRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact for academy: {AcademyId}", academyId);

        var command = new UpdateContactCommand
        {
            AcademyId = academyId,
            PrimaryContactName = request.PrimaryContactName,
            PrimaryPhone = request.PrimaryPhone,
            PrimaryEmail = request.PrimaryEmail,
            SecondaryContactName = request.SecondaryContactName,
            SecondaryPhone = request.SecondaryPhone,
            SecondaryEmail = request.SecondaryEmail,
            Address = request.Address,
            Country = request.Country,
            State = request.State,
            City = request.City,
            PostalCode = request.PostalCode,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Contact updated for academy: {AcademyId}", academyId);

        return Ok(ApiResponse<ContactDto>.SuccessResult(result.Value!, "Contact updated successfully."));
    }

    /// <summary>
    /// Updates the operating hours for an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Operating hours fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated operating hours</returns>
    /// <response code="200">Operating hours updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPut("{academyId:guid}/operating-hours")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<OperatingHoursDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateOperatingHoursRequest), typeof(UpdateOperatingHoursRequestExample))]
    public async Task<IActionResult> UpdateOperatingHours(
        Guid academyId,
        [FromBody] UpdateOperatingHoursRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating operating hours for academy: {AcademyId}", academyId);

        var command = new UpdateOperatingHoursCommand
        {
            AcademyId = academyId,
            MondayOpening = request.MondayOpening,
            MondayClosing = request.MondayClosing,
            TuesdayOpening = request.TuesdayOpening,
            TuesdayClosing = request.TuesdayClosing,
            WednesdayOpening = request.WednesdayOpening,
            WednesdayClosing = request.WednesdayClosing,
            ThursdayOpening = request.ThursdayOpening,
            ThursdayClosing = request.ThursdayClosing,
            FridayOpening = request.FridayOpening,
            FridayClosing = request.FridayClosing,
            SaturdayOpening = request.SaturdayOpening,
            SaturdayClosing = request.SaturdayClosing,
            SundayOpening = request.SundayOpening,
            SundayClosing = request.SundayClosing,
            HolidaySchedule = request.HolidaySchedule
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Operating hours updated for academy: {AcademyId}", academyId);

        return Ok(ApiResponse<OperatingHoursDto>.SuccessResult(result.Value!, "Operating hours updated successfully."));
    }

    /// <summary>
    /// Gets the operating hours for an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Operating hours</returns>
    /// <response code="200">Operating hours retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Academy not found</response>
    [HttpGet("{academyId:guid}/operating-hours")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<OperatingHoursDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOperatingHours(
        Guid academyId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching operating hours for academy: {AcademyId}", academyId);

        var result = await _mediator.Send(new GetOperatingHoursQuery { AcademyId = academyId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<OperatingHoursDto>.SuccessResult(result.Value!, "Operating hours retrieved successfully."));
    }

    #endregion

    #region Social Links

    /// <summary>
    /// Updates the social media links for an academy. Replaces all existing links.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Social links to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated social links</returns>
    /// <response code="200">Social links updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPut("{academyId:guid}/social-links")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<SocialLinkDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateSocialLinksRequest), typeof(UpdateSocialLinksRequestExample))]
    public async Task<IActionResult> UpdateSocialLinks(
        Guid academyId,
        [FromBody] UpdateSocialLinksRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating social links for academy: {AcademyId}", academyId);

        var links = request.Links.Select(l => new SocialLinkInput
        {
            Platform = l.Platform,
            Url = l.Url
        }).ToList();

        var command = new UpdateSocialLinksCommand
        {
            AcademyId = academyId,
            Links = links
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Social links updated for academy: {AcademyId}", academyId);

        return Ok(ApiResponse<IReadOnlyList<SocialLinkDto>>.SuccessResult(result.Value!, "Social links updated successfully."));
    }

    #endregion

    #region Sport Assignment

    /// <summary>
    /// Assigns a sport to an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="request">Sport assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created sport assignment</returns>
    /// <response code="201">Sport assigned successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy or sport not found</response>
    /// <response code="409">Sport already assigned to this academy</response>
    [HttpPost("{academyId:guid}/sports")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<AcademySportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(AssignAcademySportRequest), typeof(AssignAcademySportRequestExample))]
    public async Task<IActionResult> AssignSport(
        Guid academyId,
        [FromBody] AssignAcademySportRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning sport {SportId} to academy: {AcademyId}", request.SportId, academyId);

        var command = new AssignSportCommand
        {
            AcademyId = academyId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Sport assigned to academy: {AcademyId}", academyId);

        return CreatedAtAction(
            nameof(GetAcademyById),
            new { academyId, version = "1.0" },
            ApiResponse<AcademySportDto>.SuccessResult(result.Value!, "Sport assigned successfully."));
    }

    /// <summary>
    /// Removes a sport assignment from an academy.
    /// </summary>
    /// <param name="academyId">The academy's unique identifier</param>
    /// <param name="sportId">The sport's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Sport removed successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Sport assignment not found</response>
    [HttpDelete("{academyId:guid}/sports/{sportId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSport(
        Guid academyId,
        Guid sportId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing sport {SportId} from academy: {AcademyId}", sportId, academyId);

        var result = await _mediator.Send(
            new RemoveSportCommand { AcademyId = academyId, SportId = sportId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Sport removed from academy: {AcademyId}", academyId);

        return NoContent();
    }

    #endregion

    #region Helpers

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
            error.Contains("already associated", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("already assigned", StringComparison.OrdinalIgnoreCase))
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
