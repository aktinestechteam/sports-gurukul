using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.FacilityManagement.Commands.AddCourt;
using SportsGurukul.Application.Features.FacilityManagement.Commands.AddEquipment;
using SportsGurukul.Application.Features.FacilityManagement.Commands.CreateFacility;
using SportsGurukul.Application.Features.FacilityManagement.Commands.DeleteFacility;
using SportsGurukul.Application.Features.FacilityManagement.Commands.RestoreFacility;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacility;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdateFacilitySchedule;
using SportsGurukul.Application.Features.FacilityManagement.Commands.UpdatePricing;
using SportsGurukul.Application.Features.FacilityManagement.DTOs;
using SportsGurukul.Application.Features.FacilityManagement.Queries.GetFacilityById;
using SportsGurukul.Application.Features.FacilityManagement.Queries.SearchFacilities;
using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages facilities — creation, retrieval, update, deletion, courts, equipment, pricing, and scheduling.
/// </summary>
[ApiController]
[Route("api/v1/facilities")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Facility Management")]
public class FacilityController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FacilityController> _logger;

    public FacilityController(IMediator mediator, ILogger<FacilityController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new facility.
    /// </summary>
    /// <param name="request">Facility details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created facility</returns>
    /// <response code="201">Facility created successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Academy not found</response>
    [HttpPost]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDetailDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(CreateFacilityApiRequest), typeof(CreateFacilityApiRequestExample))]
    public async Task<IActionResult> CreateFacility(
        [FromBody] CreateFacilityApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating facility: {FacilityName} for academy: {AcademyId}", request.FacilityName, request.AcademyId);

        var command = new CreateFacilityCommand
        {
            AcademyId = request.AcademyId,
            BranchId = request.BranchId,
            FacilityName = request.FacilityName,
            FacilityType = request.FacilityType,
            Description = request.Description,
            Capacity = request.Capacity,
            IndoorOutdoor = request.IndoorOutdoor,
            SurfaceType = request.SurfaceType,
            LightingAvailable = request.LightingAvailable,
            ParkingAvailable = request.ParkingAvailable,
            ChangingRoomAvailable = request.ChangingRoomAvailable,
            WashroomAvailable = request.WashroomAvailable,
            MedicalRoomAvailable = request.MedicalRoomAvailable
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Facility created: {FacilityId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetFacilityById),
            new { facilityId = result.Value.Id, version = "1.0" },
            ApiResponse<FacilityDetailDto>.SuccessResult(result.Value, "Facility created successfully."));
    }

    /// <summary>
    /// Gets facilities with optional filtering and pagination.
    /// </summary>
    /// <param name="academyId">Optional academy identifier to filter by</param>
    /// <param name="facilityType">Optional facility type to filter by</param>
    /// <param name="searchTerm">Optional search term to match facility names</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of facilities</returns>
    /// <response code="200">Facilities retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilitySearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FacilitySearchResponseExample))]
    public async Task<IActionResult> GetFacilities(
        [FromQuery] Guid? academyId,
        [FromQuery] FacilityType? facilityType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching facilities - AcademyId: {AcademyId}, Page: {Page}", academyId, page);

        var query = new SearchFacilitiesQuery
        {
            AcademyId = academyId,
            FacilityType = facilityType,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<FacilitySearchResponse>.SuccessResult(result.Value!, "Facilities retrieved successfully."));
    }

    /// <summary>
    /// Gets a specific facility by its unique identifier.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Facility details</returns>
    /// <response code="200">Facility retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Facility not found</response>
    [HttpGet("{facilityId:guid}")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FacilityDetailDtoExample))]
    public async Task<IActionResult> GetFacilityById(
        Guid facilityId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching facility: {FacilityId}", facilityId);

        var result = await _mediator.Send(new GetFacilityByIdQuery { FacilityId = facilityId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<FacilityDetailDto>.SuccessResult(result.Value!, "Facility retrieved successfully."));
    }

    /// <summary>
    /// Updates a facility. All fields are optional — only supplied fields are applied.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="request">Fields to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated facility</returns>
    /// <response code="200">Facility updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    [HttpPut("{facilityId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateFacilityApiRequest), typeof(UpdateFacilityApiRequestExample))]
    public async Task<IActionResult> UpdateFacility(
        Guid facilityId,
        [FromBody] UpdateFacilityApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating facility: {FacilityId}", facilityId);

        var command = new UpdateFacilityCommand
        {
            FacilityId = facilityId,
            FacilityName = request.FacilityName,
            FacilityType = request.FacilityType,
            Description = request.Description,
            Capacity = request.Capacity,
            IndoorOutdoor = request.IndoorOutdoor,
            SurfaceType = request.SurfaceType,
            LightingAvailable = request.LightingAvailable,
            ParkingAvailable = request.ParkingAvailable,
            ChangingRoomAvailable = request.ChangingRoomAvailable,
            WashroomAvailable = request.WashroomAvailable,
            MedicalRoomAvailable = request.MedicalRoomAvailable,
            Status = request.Status
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Facility updated: {FacilityId}", facilityId);

        return Ok(ApiResponse<FacilityDetailDto>.SuccessResult(result.Value!, "Facility updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Facility deleted successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    [HttpDelete("{facilityId:guid}")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFacility(
        Guid facilityId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting facility: {FacilityId}", facilityId);

        var result = await _mediator.Send(new DeleteFacilityCommand { FacilityId = facilityId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Facility deleted: {FacilityId}", facilityId);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously soft-deleted facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Restored facility</returns>
    /// <response code="200">Facility restored successfully</response>
    /// <response code="400">Facility is not deleted or cannot be restored</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">No deleted facility found with this ID</response>
    [HttpPost("{facilityId:guid}/restore")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilityDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RestoreFacility(
        Guid facilityId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring facility: {FacilityId}", facilityId);

        var result = await _mediator.Send(new RestoreFacilityCommand { FacilityId = facilityId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Facility restored: {FacilityId}", facilityId);

        return Ok(ApiResponse<FacilityDetailDto>.SuccessResult(result.Value!, "Facility restored successfully."));
    }

    /// <summary>
    /// Adds a court to a facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="request">Court details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created court</returns>
    /// <response code="201">Court added successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    /// <response code="409">Court number already exists in this facility</response>
    [HttpPost("{facilityId:guid}/courts")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<CourtDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [SwaggerRequestExample(typeof(AddCourtApiRequest), typeof(AddCourtApiRequestExample))]
    public async Task<IActionResult> AddCourt(
        Guid facilityId,
        [FromBody] AddCourtApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding court to facility: {FacilityId}", facilityId);

        var command = new AddCourtCommand
        {
            FacilityId = facilityId,
            CourtNumber = request.CourtNumber,
            CourtName = request.CourtName,
            CourtType = request.CourtType,
            Capacity = request.Capacity,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Court added to facility: {FacilityId}", facilityId);

        return CreatedAtAction(
            nameof(GetFacilityById),
            new { facilityId, version = "1.0" },
            ApiResponse<CourtDto>.SuccessResult(result.Value!, "Court added successfully."));
    }

    /// <summary>
    /// Updates the pricing for a facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="request">Pricing details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated pricing</returns>
    /// <response code="200">Pricing updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    [HttpPut("{facilityId:guid}/pricing")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<PricingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdatePricingApiRequest), typeof(UpdatePricingApiRequestExample))]
    public async Task<IActionResult> UpdatePricing(
        Guid facilityId,
        [FromBody] UpdatePricingApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating pricing for facility: {FacilityId}", facilityId);

        var command = new UpdatePricingCommand
        {
            FacilityId = facilityId,
            PricingName = request.PricingName,
            HourlyRate = request.HourlyRate,
            DailyRate = request.DailyRate,
            MonthlyRate = request.MonthlyRate,
            PeakHourlyRate = request.PeakHourlyRate,
            OffPeakHourlyRate = request.OffPeakHourlyRate,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Pricing updated for facility: {FacilityId}", facilityId);

        return Ok(ApiResponse<PricingDto>.SuccessResult(result.Value!, "Pricing updated successfully."));
    }

    /// <summary>
    /// Updates the schedule for a facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="request">Schedule details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated schedule</returns>
    /// <response code="200">Schedule updated successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    [HttpPut("{facilityId:guid}/schedule")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(UpdateScheduleApiRequest), typeof(UpdateScheduleApiRequestExample))]
    public async Task<IActionResult> UpdateFacilitySchedule(
        Guid facilityId,
        [FromBody] UpdateScheduleApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating schedule for facility: {FacilityId}", facilityId);

        var command = new UpdateFacilityScheduleCommand
        {
            FacilityId = facilityId,
            DayOfWeek = request.DayOfWeek,
            OpeningTime = request.OpeningTime,
            ClosingTime = request.ClosingTime,
            IsClosed = request.IsClosed,
            IsMaintenanceWindow = request.IsMaintenanceWindow,
            Notes = request.Notes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Schedule updated for facility: {FacilityId}", facilityId);

        return Ok(ApiResponse<ScheduleDto>.SuccessResult(result.Value!, "Schedule updated successfully."));
    }

    /// <summary>
    /// Adds equipment to a facility.
    /// </summary>
    /// <param name="facilityId">The facility's unique identifier</param>
    /// <param name="request">Equipment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created equipment</returns>
    /// <response code="201">Equipment added successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Facility not found</response>
    [HttpPost("{facilityId:guid}/equipment")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<EquipmentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(AddEquipmentApiRequest), typeof(AddEquipmentApiRequestExample))]
    public async Task<IActionResult> AddEquipment(
        Guid facilityId,
        [FromBody] AddEquipmentApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding equipment to facility: {FacilityId}", facilityId);

        var command = new AddEquipmentCommand
        {
            FacilityId = facilityId,
            EquipmentName = request.EquipmentName,
            Category = request.Category,
            PurchaseDate = request.PurchaseDate,
            Condition = request.Condition,
            MaintenanceSchedule = request.MaintenanceSchedule,
            WarrantyExpiry = request.WarrantyExpiry,
            Quantity = request.Quantity,
            Description = request.Description
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Equipment added to facility: {FacilityId}", facilityId);

        return CreatedAtAction(
            nameof(GetFacilityById),
            new { facilityId, version = "1.0" },
            ApiResponse<EquipmentDto>.SuccessResult(result.Value!, "Equipment added successfully."));
    }

    /// <summary>
    /// Searches facilities with advanced filtering and pagination.
    /// </summary>
    /// <param name="academyId">Optional academy identifier to filter by</param>
    /// <param name="facilityType">Optional facility type to filter by</param>
    /// <param name="searchTerm">Optional search term to match facility names</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of facilities matching search criteria</returns>
    /// <response code="200">Search results retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("search")]
    [Authorize(Roles = "Academy Admin,Coach,Athlete,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<FacilitySearchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FacilitySearchResponseExample))]
    public async Task<IActionResult> SearchFacilities(
        [FromQuery] Guid? academyId,
        [FromQuery] FacilityType? facilityType,
        [FromQuery] string? searchTerm,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Searching facilities - Term: {SearchTerm}, Page: {Page}", searchTerm, page);

        var query = new SearchFacilitiesQuery
        {
            AcademyId = academyId,
            FacilityType = facilityType,
            SearchTerm = searchTerm,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<FacilitySearchResponse>.SuccessResult(result.Value!, "Facilities search completed successfully."));
    }

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
