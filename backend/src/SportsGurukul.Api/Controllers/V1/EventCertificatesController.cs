using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.EventManagement.Commands.GenerateCertificates;
using SportsGurukul.Application.Features.EventManagement.Commands.IssueCertificate;
using SportsGurukul.Application.Features.EventManagement.Commands.RevokeCertificate;
using SportsGurukul.Application.Features.EventManagement.DTOs;
using SportsGurukul.Application.Features.EventManagement.Queries.GetCertificatesByEvent;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages event certificates — generation, issuance, revocation, and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/events/{eventId}/certificates")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Event Certificates")]
public class EventCertificatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventCertificatesController> _logger;

    public EventCertificatesController(IMediator mediator, ILogger<EventCertificatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Generates certificates for all eligible participants of an event.
    /// </summary>
    [HttpPost("generate")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<List<CertificateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GenerateCertificates(
        [FromRoute] Guid eventId,
        [FromBody] GenerateCertificatesCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating certificates for event: {EventId}", eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Certificates generated for event: {EventId}", eventId);

        return Ok(ApiResponse<List<CertificateDto>>.SuccessResult(result.Value!, "Certificates generated successfully."));
    }

    /// <summary>
    /// Issues a specific certificate to a participant.
    /// </summary>
    [HttpPost("{certificateId:guid}/issue")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> IssueCertificate(
        [FromRoute] Guid eventId,
        [FromRoute] Guid certificateId,
        [FromBody] IssueCertificateCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing certificate: {CertificateId} for event: {EventId}", certificateId, eventId);

        command.EventId = eventId;

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Certificate issued: {CertificateId}", certificateId);

        return CreatedAtAction(
            nameof(GetCertificates),
            new { eventId },
            ApiResponse<CertificateDto>.SuccessResult(result.Value!, "Certificate issued successfully."));
    }

    /// <summary>
    /// Revokes a previously issued certificate.
    /// </summary>
    [HttpPost("{certificateId:guid}/revoke")]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RevokeCertificate(
        [FromRoute] Guid eventId,
        [FromRoute] Guid certificateId,
        [FromBody] RevokeCertificateRequest? request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking certificate: {CertificateId} for event: {EventId}", certificateId, eventId);

        var command = new RevokeCertificateCommand
        {
            CertificateId = certificateId,
            Reason = request?.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Certificate revoked: {CertificateId}", certificateId);

        return Ok(ApiResponse<CertificateDto>.SuccessResult(result.Value!, "Certificate revoked successfully."));
    }

    /// <summary>
    /// Retrieves all certificates for a specific event.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "System Admin,Academy Admin,Event Manager")]
    [ProducesResponseType(typeof(ApiResponse<List<CertificateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetCertificates(
        [FromRoute] Guid eventId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving certificates for event: {EventId}", eventId);

        var query = new GetCertificatesByEventQuery { EventId = eventId };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<List<CertificateDto>>.SuccessResult(result.Value!, "Certificates retrieved successfully."));
    }

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("conflict", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }
}

public record RevokeCertificateRequest(string? Reason);
