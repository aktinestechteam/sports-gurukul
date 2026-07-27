using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.SwaggerExamples;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ResolveBookingConflict;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Commands.ValidateBookingConflict;
using SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Queries.GetBookingConflicts;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Detects, lists, and resolves booking conflicts (coach overlap, facility overlap, athlete overlap, etc.).
/// </summary>
[ApiController]
[Route("api/v1/bookings/conflicts")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Booking Conflicts")]
public class BookingConflictsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BookingConflictsController> _logger;

    public BookingConflictsController(IMediator mediator, ILogger<BookingConflictsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets all conflicts for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier to query conflicts for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of booking conflicts</returns>
    /// <response code="200">Conflicts retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="404">Booking not found</response>
    [HttpGet]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingConflictDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBookingConflicts(
        [FromQuery] Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching conflicts for booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new GetBookingConflictsQuery { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingConflictDto>>.SuccessResult(result.Value!, "Booking conflicts retrieved successfully."));
    }

    /// <summary>
    /// Validates and detects conflicts for a specific booking.
    /// </summary>
    /// <param name="bookingId">The booking's unique identifier to validate conflicts for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of detected conflicts</returns>
    /// <response code="200">Conflict validation completed</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Booking not found</response>
    [HttpPost("validate")]
    [Authorize(Roles = "Academy Admin,Coach,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<BookingConflictDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ValidateBookingConflict(
        [FromQuery] Guid bookingId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Validating conflicts for booking: {BookingId}", bookingId);

        var result = await _mediator.Send(new ValidateBookingConflictCommand { BookingId = bookingId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<BookingConflictDto>>.SuccessResult(result.Value!, "Conflict validation completed."));
    }

    /// <summary>
    /// Resolves a booking conflict with resolution notes.
    /// </summary>
    /// <param name="conflictId">The conflict's unique identifier</param>
    /// <param name="request">Resolution details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Resolution status</returns>
    /// <response code="200">Conflict resolved successfully</response>
    /// <response code="400">Conflict already resolved or validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Conflict not found</response>
    [HttpPost("{conflictId:guid}/resolve")]
    [Authorize(Roles = "Academy Admin,System Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [SwaggerRequestExample(typeof(ResolveBookingConflictApiRequest), typeof(ResolveBookingConflictApiRequestExample))]
    public async Task<IActionResult> ResolveBookingConflict(
        Guid conflictId,
        [FromBody] ResolveBookingConflictApiRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resolving conflict: {ConflictId}", conflictId);

        var command = new ResolveBookingConflictCommand
        {
            ConflictId = conflictId,
            ResolutionNotes = request.ResolutionNotes
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Conflict resolved: {ConflictId}", conflictId);

        return Ok(ApiResponse<bool>.SuccessResult(result.Value!, "Conflict resolved successfully."));
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
