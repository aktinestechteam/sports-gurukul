using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.IssueCertificate;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificateByIdQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.Queries.GetCertificatesByAthleteQuery;
using SportsGurukul.Application.Features.TrainingProgramManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages training certificate issuance, retrieval, and athlete certificate history.
/// </summary>
[ApiController]
[Route("api/v1/certificates")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
[Tags("Certificates")]
public class CertificatesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CertificatesController> _logger;

    public CertificatesController(IMediator mediator, ILogger<CertificatesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Request Types

    public record IssueCertificateRequest
    {
        public string CertificateType { get; init; } = string.Empty;
        public string? FileUrl { get; init; }
    }

    #endregion

    #region Certificates

    /// <summary>
    /// Issues a training certificate for an enrollment.
    /// </summary>
    /// <param name="enrollmentId">The enrollment's unique identifier</param>
    /// <param name="request">Certificate details including type and optional file URL</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created certificate</returns>
    /// <response code="201">Certificate issued successfully</response>
    /// <response code="400">Validation error</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Enrollment not found</response>
    /// <response code="409">Certificate already issued for this enrollment</response>
    [HttpPost("~/api/v1/enrollments/{enrollmentId:guid}/certificate")]
    [Authorize(Roles = "Coach,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> IssueCertificate(
        Guid enrollmentId,
        [FromBody] IssueCertificateRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Issuing certificate for enrollment {EnrollmentId}: {CertificateType}", enrollmentId, request.CertificateType);

        var command = new IssueCertificateCommand
        {
            EnrollmentId = enrollmentId,
            CertificateType = request.CertificateType,
            FileUrl = request.FileUrl
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        _logger.LogInformation("Certificate issued: {CertificateId}", result.Value!.Id);

        return CreatedAtAction(
            nameof(GetCertificate),
            new { certificateId = result.Value.Id, version = "1.0" },
            ApiResponse<CertificateDto>.SuccessResult(result.Value, "Certificate issued successfully."));
    }

    /// <summary>
    /// Gets a certificate by its unique identifier.
    /// </summary>
    /// <param name="certificateId">The certificate's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Certificate details</returns>
    /// <response code="200">Certificate retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Certificate not found</response>
    [HttpGet("{certificateId:guid}")]
    [Authorize(Roles = "Athlete,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<CertificateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCertificate(
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching certificate: {CertificateId}", certificateId);

        var result = await _mediator.Send(new GetCertificateByIdQuery { Id = certificateId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<CertificateDto>.SuccessResult(result.Value!, "Certificate retrieved successfully."));
    }

    /// <summary>
    /// Gets all certificates issued to a specific athlete.
    /// </summary>
    /// <param name="athleteId">The athlete's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of certificates for the athlete</returns>
    /// <response code="200">Certificates retrieved successfully</response>
    /// <response code="401">Not authenticated</response>
    /// <response code="403">Insufficient permissions</response>
    /// <response code="404">Athlete not found</response>
    [HttpGet("~/api/v1/athletes/{athleteId:guid}/certificates")]
    [Authorize(Roles = "Athlete,System Admin,Academy Admin")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<CertificateDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAthleteCertificates(
        Guid athleteId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching certificates for athlete {AthleteId}", athleteId);

        var result = await _mediator.Send(new GetCertificatesByAthleteQuery { AthleteId = athleteId }, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<CertificateDto>>.SuccessResult(result.Value!, "Certificates retrieved successfully."));
    }

    #endregion

    #region Helpers

    private IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = "Not Found", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4" });
        if (error.Contains("already exists", StringComparison.OrdinalIgnoreCase) || error.Contains("already", StringComparison.OrdinalIgnoreCase))
            return Conflict(new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = "Conflict", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8" });
        return BadRequest(new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = "Bad Request", Detail = error, Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1" });
    }

    #endregion
}
