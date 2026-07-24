using System.Net;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignSport;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddCertification;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCertification;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCertification;
using SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCertification;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddExperience;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateExperience;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteExperience;
using SportsGurukul.Application.Features.CoachManagement.Commands.AddEducation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateEducation;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteEducation;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateLocation;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveAthlete;
using SportsGurukul.Application.Features.CoachManagement.Commands.CreateCoach;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoach;
using SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoach;
using SportsGurukul.Application.Features.CoachManagement.Commands.ActivateCoach;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeactivateCoach;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachProfile;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachById;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachByUserId;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachProfile;
using SportsGurukul.Application.Features.CoachManagement.Queries.SearchCoaches;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSports;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachAvailability;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachCertifications;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachExperience;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachEducation;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetAssignedAthletes;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetPagedCoaches;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Authorize]
public class CoachController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoachController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #region Profile Management

    /// <summary>
    /// Create a new coach profile linked to an existing user account.
    /// </summary>
    /// <param name="request">Coach creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created coach profile.</returns>
    /// <response code="201">Coach profile created successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to create coach profiles.</response>
    /// <response code="409">Coach profile already exists for this user.</response>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [SwaggerRequestExample(typeof(CreateCoachRequest), typeof(CreateCoachRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(CoachDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> CreateCoach(
        [FromBody] CreateCoachRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCoachCommand
        {
            UserId = request.UserId,
            Biography = request.Biography,
            YearsOfExperience = request.YearsOfExperience,
            CurrentOrganization = request.CurrentOrganization,
            HighestQualification = request.HighestQualification,
            PreferredLanguage = request.PreferredLanguage,
            CoachingLevel = request.CoachingLevel
        };

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetCoachProfile),
            new { coachId = result.Value!.Id },
            new ApiResponse<CoachDto>
            {
                Success = true,
                Message = "Coach profile created successfully.",
                Data = result.Value
            });
    }

    /// <summary>
    /// Get a coach's full profile with all sub-profiles (sports, certifications, experience, education, availability, location, assigned athletes).
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Complete coach profile.</returns>
    /// <response code="200">Coach profile found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachProfile(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachProfileQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<CoachProfileDto>
        {
            Success = true,
            Message = "Coach profile retrieved successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Get a coach by their unique identifier.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Coach details.</returns>
    /// <response code="200">Coach found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/details")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachById(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachByIdQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<CoachDto>
        {
            Success = true,
            Message = "Coach retrieved successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Get a coach profile by the linked user ID.
    /// </summary>
    /// <param name="userId">User unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Coach details.</returns>
    /// <response code="200">Coach found.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Coach not found for this user.</response>
    [HttpGet("user/{userId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachByUserId(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<CoachDto>
        {
            Success = true,
            Message = "Coach retrieved successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Update an existing coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Coach update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated coach profile.</returns>
    /// <response code="200">Coach profile updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to update this coach profile.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPut("{coachId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateCoachProfileRequest), typeof(UpdateCoachProfileRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateCoachProfile(
        Guid coachId,
        [FromBody] UpdateCoachProfileRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCoachProfileCommand
        {
            CoachId = coachId,
            Biography = request.Biography,
            YearsOfExperience = request.YearsOfExperience,
            CurrentOrganization = request.CurrentOrganization,
            HighestQualification = request.HighestQualification,
            PreferredLanguage = request.PreferredLanguage,
            CoachingLevel = request.CoachingLevel
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<CoachDto>
        {
            Success = true,
            Message = "Coach profile updated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Soft-delete a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Coach profile deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to delete this coach profile.</response>
    /// <response code="404">Coach not found.</response>
    [HttpDelete("{coachId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> DeleteCoach(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCoachCommand { CoachId = coachId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Coach profile deleted successfully."
        });
    }

    /// <summary>
    /// Restore a soft-deleted coach profile. Only Academy Admin or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Coach profile restored successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to restore coach profiles.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPost("{coachId}/restore")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Academy Admin,System Admin")]
    public async Task<IActionResult> RestoreCoach(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var command = new RestoreCoachCommand { CoachId = coachId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Coach profile restored successfully."
        });
    }

    #endregion

    #region Sport Management

    /// <summary>
    /// Assign a sport to a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Sport assignment details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created sport assignment.</returns>
    /// <response code="201">Sport assigned successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's sports.</response>
    /// <response code="404">Coach or sport not found.</response>
    /// <response code="409">Sport already assigned to this coach.</response>
    [HttpPost("{coachId}/sports")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [SwaggerRequestExample(typeof(CoachAssignSportRequest), typeof(CoachAssignSportRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(CoachSportDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> AssignSport(
        Guid coachId,
        [FromBody] CoachAssignSportRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AssignSportCommand
        {
            CoachId = coachId,
            SportId = request.SportId,
            IsPrimarySport = request.IsPrimarySport
        };
        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetCoachSports),
            new { coachId },
            new ApiResponse<SportDto>
            {
                Success = true,
                Message = "Sport assigned successfully.",
                Data = result.Value!
            });
    }

    /// <summary>
    /// Remove a sport assignment from a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="sportId">Sport unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Sport removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's sports.</response>
    /// <response code="404">Coach or sport assignment not found.</response>
    [HttpDelete("{coachId}/sports/{sportId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> RemoveSport(
        Guid coachId,
        Guid sportId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveSportCommand { CoachId = coachId, SportId = sportId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Sport removed successfully."
        });
    }

    /// <summary>
    /// Get all sports assigned to a coach. Public endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of assigned sports.</returns>
    /// <response code="200">Sports retrieved successfully.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/sports")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachSportDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachSports(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachSportsQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<SportDto>>
        {
            Success = true,
            Message = "Coach sports retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Certification Management

    /// <summary>
    /// Add a certification to a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Certification details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created certification.</returns>
    /// <response code="201">Certification added successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's certifications.</response>
    /// <response code="404">Coach not found.</response>
    /// <response code="409">Certification with same name already exists.</response>
    [HttpPost("{coachId}/certifications")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [SwaggerRequestExample(typeof(AddCertificationRequest), typeof(AddCertificationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(CertificationDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> AddCertification(
        Guid coachId,
        [FromBody] AddCertificationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCertificationCommand
        {
            CoachId = coachId,
            CertificationName = request.CertificationName,
            IssuingAuthority = request.IssuingAuthority,
            CertificateNumber = request.CertificateNumber,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            CertificateUrl = request.CertificateUrl
        };

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetCoachCertifications),
            new { coachId },
            new ApiResponse<CertificationDto>
            {
                Success = true,
                Message = "Certification added successfully.",
                Data = result.Value!
            });
    }

    /// <summary>
    /// Update a certification for a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="certificationId">Certification unique identifier.</param>
    /// <param name="request">Certification update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated certification.</returns>
    /// <response code="200">Certification updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's certifications.</response>
    /// <response code="404">Certification not found.</response>
    [HttpPut("{coachId}/certifications/{certificationId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateCertificationRequest), typeof(UpdateCertificationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CertificationDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateCertification(
        Guid coachId,
        Guid certificationId,
        [FromBody] UpdateCertificationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCertificationCommand
        {
            CertificationId = certificationId,
            CertificationName = request.CertificationName,
            IssuingAuthority = request.IssuingAuthority,
            CertificateNumber = request.CertificateNumber,
            IssueDate = request.IssueDate,
            ExpiryDate = request.ExpiryDate,
            CertificateUrl = request.CertificateUrl
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<CertificationDto>
        {
            Success = true,
            Message = "Certification updated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Delete a certification from a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="certificationId">Certification unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Certification deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's certifications.</response>
    /// <response code="404">Certification not found.</response>
    [HttpDelete("{coachId}/certifications/{certificationId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> DeleteCertification(
        Guid coachId,
        Guid certificationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCertificationCommand { CertificationId = certificationId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Certification deleted successfully."
        });
    }

    /// <summary>
    /// Get all certifications for a coach. Public endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of certifications.</returns>
    /// <response code="200">Certifications retrieved successfully.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/certifications")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CertificationDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachCertifications(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachCertificationsQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<CertificationDto>>
        {
            Success = true,
            Message = "Coach certifications retrieved successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Verify or reject a coach certification. Only Academy Admin or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="certificationId">Certification unique identifier.</param>
    /// <param name="request">Verification status to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated certification.</returns>
    /// <response code="200">Certification verified successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to verify certifications.</response>
    /// <response code="404">Certification not found.</response>
    [HttpPost("{coachId}/certifications/{certificationId}/verify")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(VerifyCertificationRequest), typeof(VerifyCertificationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CertificationDtoExample))]
    [Authorize(Roles = "Academy Admin,System Admin")]
    public async Task<IActionResult> VerifyCertification(
        Guid coachId,
        Guid certificationId,
        [FromBody] VerifyCertificationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new VerifyCertificationCommand
        {
            CertificationId = certificationId,
            Status = request.Status
        };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<CertificationDto>
        {
            Success = true,
            Message = "Certification verification updated successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Experience Management

    /// <summary>
    /// Add an experience entry to a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Experience details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created experience entry.</returns>
    /// <response code="201">Experience added successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's experience.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPost("{coachId}/experience")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(AddExperienceRequest), typeof(AddExperienceRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(ExperienceDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> AddExperience(
        Guid coachId,
        [FromBody] AddExperienceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddExperienceCommand
        {
            CoachId = coachId,
            Organization = request.Organization,
            Role = request.Role,
            Sport = request.Sport,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetCoachExperience),
            new { coachId },
            new ApiResponse<ExperienceDto>
            {
                Success = true,
                Message = "Experience added successfully.",
                Data = result.Value!
            });
    }

    /// <summary>
    /// Update an experience entry for a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="experienceId">Experience unique identifier.</param>
    /// <param name="request">Experience update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated experience entry.</returns>
    /// <response code="200">Experience updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's experience.</response>
    /// <response code="404">Experience not found.</response>
    [HttpPut("{coachId}/experience/{experienceId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateExperienceRequest), typeof(UpdateExperienceRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ExperienceDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateExperience(
        Guid coachId,
        Guid experienceId,
        [FromBody] UpdateExperienceRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateExperienceCommand
        {
            ExperienceId = experienceId,
            Organization = request.Organization,
            Role = request.Role,
            Sport = request.Sport,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<ExperienceDto>
        {
            Success = true,
            Message = "Experience updated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Delete an experience entry from a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="experienceId">Experience unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Experience deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's experience.</response>
    /// <response code="404">Experience not found.</response>
    [HttpDelete("{coachId}/experience/{experienceId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> DeleteExperience(
        Guid coachId,
        Guid experienceId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteExperienceCommand { ExperienceId = experienceId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Experience deleted successfully."
        });
    }

    /// <summary>
    /// Get all experience entries for a coach. Public endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of experience entries.</returns>
    /// <response code="200">Experience entries retrieved successfully.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/experience")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(ExperienceDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachExperience(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachExperienceQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<ExperienceDto>>
        {
            Success = true,
            Message = "Coach experience retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Education Management

    /// <summary>
    /// Add an education entry to a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Education details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created education entry.</returns>
    /// <response code="201">Education added successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's education.</response>
    /// <response code="404">Coach not found.</response>
    /// <response code="409">Education with same degree and institution already exists.</response>
    [HttpPost("{coachId}/education")]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [SwaggerRequestExample(typeof(AddEducationRequest), typeof(AddEducationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.Created, typeof(EducationDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> AddEducation(
        Guid coachId,
        [FromBody] AddEducationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddEducationCommand
        {
            CoachId = coachId,
            Degree = request.Degree,
            Institution = request.Institution,
            FieldOfStudy = request.FieldOfStudy,
            YearCompleted = request.YearCompleted
        };

        var result = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetCoachEducation),
            new { coachId },
            new ApiResponse<EducationDto>
            {
                Success = true,
                Message = "Education added successfully.",
                Data = result.Value!
            });
    }

    /// <summary>
    /// Update an education entry for a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="educationId">Education unique identifier.</param>
    /// <param name="request">Education update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated education entry.</returns>
    /// <response code="200">Education updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's education.</response>
    /// <response code="404">Education not found.</response>
    [HttpPut("{coachId}/education/{educationId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateEducationRequest), typeof(UpdateEducationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(EducationDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateEducation(
        Guid coachId,
        Guid educationId,
        [FromBody] UpdateEducationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateEducationCommand
        {
            EducationId = educationId,
            Degree = request.Degree,
            Institution = request.Institution,
            FieldOfStudy = request.FieldOfStudy,
            YearCompleted = request.YearCompleted
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<EducationDto>
        {
            Success = true,
            Message = "Education updated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Delete an education entry from a coach profile. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="educationId">Education unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Education deleted successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's education.</response>
    /// <response code="404">Education not found.</response>
    [HttpDelete("{coachId}/education/{educationId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> DeleteEducation(
        Guid coachId,
        Guid educationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteEducationCommand { EducationId = educationId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Education deleted successfully."
        });
    }

    /// <summary>
    /// Get all education entries for a coach. Public endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of education entries.</returns>
    /// <response code="200">Education entries retrieved successfully.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/education")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(EducationDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachEducation(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachEducationQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<EducationDto>>
        {
            Success = true,
            Message = "Coach education retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Availability Management

    /// <summary>
    /// Update a coach's availability schedule. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Availability update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated availability.</returns>
    /// <response code="200">Availability updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's availability.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPut("{coachId}/availability")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateAvailabilityRequest), typeof(UpdateAvailabilityRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AvailabilityDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateAvailability(
        Guid coachId,
        [FromBody] UpdateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAvailabilityCommand
        {
            CoachId = coachId,
            WeeklySchedule = request.WeeklySchedule,
            TimeSlots = request.TimeSlots,
            OnlineAvailable = request.OnlineAvailable,
            OfflineAvailable = request.OfflineAvailable,
            TravelDistance = request.TravelDistance
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<AvailabilityDto>
        {
            Success = true,
            Message = "Availability updated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Get a coach's availability schedule. Public endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Availability details.</returns>
    /// <response code="200">Availability retrieved successfully.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/availability")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AvailabilityDtoExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetCoachAvailability(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetCoachAvailabilityQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<AvailabilityDto>
        {
            Success = true,
            Message = "Coach availability retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Location Management

    /// <summary>
    /// Update a coach's location. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="request">Location update details (all fields optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated location.</returns>
    /// <response code="200">Location updated successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's location.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPut("{coachId}/location")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerRequestExample(typeof(UpdateLocationRequest), typeof(UpdateLocationRequestExample))]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(LocationDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> UpdateLocation(
        Guid coachId,
        [FromBody] UpdateLocationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand
        {
            CoachId = coachId,
            Country = request.Country,
            State = request.State,
            City = request.City,
            District = request.District,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<LocationDto>
        {
            Success = true,
            Message = "Location updated successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Athlete Assignment

    /// <summary>
    /// Assign an athlete to a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="athleteId">Athlete unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Assigned athlete details.</returns>
    /// <response code="200">Athlete assigned successfully.</response>
    /// <response code="400">Validation error or invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's athlete assignments.</response>
    /// <response code="404">Coach or athlete not found.</response>
    /// <response code="409">Athlete already assigned to this coach.</response>
    [HttpPost("{coachId}/athletes/{athleteId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AssignedAthleteDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> AssignAthlete(
        Guid coachId,
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        var command = new AssignAthleteCommand { CoachId = coachId, AthleteId = athleteId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<AssignedAthleteDto>
        {
            Success = true,
            Message = "Athlete assigned successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Remove an athlete assignment from a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="athleteId">Athlete unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Athlete removed successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to modify this coach's athlete assignments.</response>
    /// <response code="404">Coach or athlete assignment not found.</response>
    [HttpDelete("{coachId}/athletes/{athleteId}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(MessageResponseExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> RemoveAthlete(
        Guid coachId,
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveAthleteCommand { CoachId = coachId, AthleteId = athleteId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<MessageResponse>
        {
            Success = true,
            Message = "Athlete removed successfully."
        });
    }

    /// <summary>
    /// Get all athletes assigned to a coach. Only the coach owner, Academy Admin, or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of assigned athletes.</returns>
    /// <response code="200">Athletes retrieved successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to view this coach's athlete assignments.</response>
    /// <response code="404">Coach not found.</response>
    [HttpGet("{coachId}/athletes")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(AssignedAthleteDtoExample))]
    [Authorize(Roles = "Coach,Academy Admin,System Admin")]
    public async Task<IActionResult> GetAssignedAthletes(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var query = new GetAssignedAthletesQuery { CoachId = coachId };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<IReadOnlyList<AssignedAthleteDto>>
        {
            Success = true,
            Message = "Assigned athletes retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Search & Paged Queries

    /// <summary>
    /// Search coaches by keyword with optional filters. Public endpoint.
    /// </summary>
    /// <param name="request">Search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    /// <response code="200">Search results returned.</response>
    [HttpGet("search")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachSearchResponseExample))]
    [AllowAnonymous]
    public async Task<IActionResult> SearchCoaches(
        [FromQuery] SearchCoachesRequest request,
        CancellationToken cancellationToken)
    {
        var query = new SearchCoachesQuery
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            SportName = request.SportName,
            CertificationName = request.CertificationName,
            MinExperience = request.MinExperience,
            MaxExperience = request.MaxExperience,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Language = request.Language,
            OnlineAvailable = request.OnlineAvailable,
            OfflineAvailable = request.OfflineAvailable,
            CoachingLevel = request.CoachingLevel,
            Status = request.Status,
            VerificationStatus = request.VerificationStatus,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<CoachSearchResponse>
        {
            Success = true,
            Message = "Coach search completed successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Get a paged list of coaches with optional sorting. Public endpoint.
    /// </summary>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="pageSize">Items per page (default: 20).</param>
    /// <param name="sortBy">Sort field (name, experience, rating).</param>
    /// <param name="sortDescending">Sort descending flag.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paged list of coaches.</returns>
    /// <response code="200">Paged coaches retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachSearchResponseExample))]
    [AllowAnonymous]
    public async Task<IActionResult> GetPagedCoaches(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = false,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPagedCoachesQuery
        {
            Page = page,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending
        };

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(new ApiResponse<CoachSearchResponse>
        {
            Success = true,
            Message = "Coaches retrieved successfully.",
            Data = result.Value!
        });
    }

    #endregion

    #region Status Management

    /// <summary>
    /// Activate a coach profile. Only Academy Admin or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Coach activated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to activate coaches.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPost("{coachId}/activate")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [Authorize(Roles = "Academy Admin,System Admin")]
    public async Task<IActionResult> ActivateCoach(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var command = new ActivateCoachCommand { CoachId = coachId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<CoachDto>
        {
            Success = true,
            Message = "Coach activated successfully.",
            Data = result.Value!
        });
    }

    /// <summary>
    /// Deactivate a coach profile. Only Academy Admin or System Admin may call this endpoint.
    /// </summary>
    /// <param name="coachId">Coach unique identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Coach deactivated successfully.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User is not authorized to deactivate coaches.</response>
    /// <response code="404">Coach not found.</response>
    [HttpPost("{coachId}/deactivate")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.Forbidden)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CoachDtoExample))]
    [Authorize(Roles = "Academy Admin,System Admin")]
    public async Task<IActionResult> DeactivateCoach(
        Guid coachId,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateCoachCommand { CoachId = coachId };
        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse<CoachDto>
        {
            Success = true,
            Message = "Coach deactivated successfully.",
            Data = result.Value!
        });
    }

    #endregion
}

/// <summary>
/// Request model for coach search with full filter options.
/// </summary>
public class SearchCoachesRequest
{
    /// <summary>Search term for full-text search across name, specialization, location.</summary>
    /// <example>cricket</example>
    public string? SearchTerm { get; set; }

    /// <summary>Filter by coach name.</summary>
    /// <example>Rajesh</example>
    public string? Name { get; set; }

    /// <summary>Filter by sport name.</summary>
    /// <example>Cricket</example>
    public string? SportName { get; set; }

    /// <summary>Filter by certification name.</summary>
    /// <example>BCCI Level A</example>
    public string? CertificationName { get; set; }

    /// <summary>Minimum years of experience.</summary>
    /// <example>5</example>
    public int? MinExperience { get; set; }

    /// <summary>Maximum years of experience.</summary>
    /// <example>15</example>
    public int? MaxExperience { get; set; }

    /// <summary>Filter by city.</summary>
    /// <example>Mumbai</example>
    public string? City { get; set; }

    /// <summary>Filter by state.</summary>
    /// <example>Maharashtra</example>
    public string? State { get; set; }

    /// <summary>Filter by country.</summary>
    /// <example>India</example>
    public string? Country { get; set; }

    /// <summary>Filter by language.</summary>
    /// <example>English</example>
    public string? Language { get; set; }

    /// <summary>Filter by online availability.</summary>
    /// <example>true</example>
    public bool? OnlineAvailable { get; set; }

    /// <summary>Filter by offline availability.</summary>
    /// <example>true</example>
    public bool? OfflineAvailable { get; set; }

    /// <summary>Filter by coaching level.</summary>
    /// <example>Senior</example>
    public CoachingLevel? CoachingLevel { get; set; }

    /// <summary>Filter by coach status.</summary>
    /// <example>Active</example>
    public CoachStatus? Status { get; set; }

    /// <summary>Filter by verification status.</summary>
    /// <example>Verified</example>
    public VerificationStatus? VerificationStatus { get; set; }

    /// <summary>Filter by registration date from.</summary>
    /// <example>2024-01-01</example>
    public DateTime? CreatedFrom { get; set; }

    /// <summary>Filter by registration date to.</summary>
    /// <example>2025-12-31</example>
    public DateTime? CreatedTo { get; set; }

    /// <summary>Sort field (name, experience, rating).</summary>
    /// <example>experience</example>
    public string? SortBy { get; set; }

    /// <summary>Sort descending flag.</summary>
    /// <example>false</example>
    public bool SortDescending { get; set; }

    /// <summary>Page number.</summary>
    /// <example>1</example>
    public int Page { get; set; } = 1;

    /// <summary>Items per page.</summary>
    /// <example>20</example>
    public int PageSize { get; set; } = 20;
}
