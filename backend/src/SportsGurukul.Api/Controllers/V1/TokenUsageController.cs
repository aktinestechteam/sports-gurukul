using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.TokenUsage;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Tracks and reports AI token usage and costs.
/// </summary>
[ApiController]
[Route("api/v1/ai/token-usage")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Token Usage")]
public class TokenUsageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TokenUsageController> _logger;

    public TokenUsageController(IMediator mediator, ILogger<TokenUsageController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Records a token usage event.
    /// </summary>
    /// <param name="command">Token usage details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The recorded token usage</returns>
    /// <response code="200">Token usage recorded successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TokenUsageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RecordTokenUsage(
        [FromBody] RecordTokenUsageCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recording token usage: {UsageType}", command.UsageType);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TokenUsageDto>.SuccessResult(result.Value!, "Token usage recorded successfully."));
    }

    /// <summary>
    /// Searches token usage records with optional filters and pagination.
    /// </summary>
    /// <param name="assistantId">Filter by assistant</param>
    /// <param name="conversationId">Filter by conversation</param>
    /// <param name="userId">Filter by user</param>
    /// <param name="usageType">Filter by usage type</param>
    /// <param name="from">Start of the date range</param>
    /// <param name="to">End of the date range</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of token usage records</returns>
    /// <response code="200">Token usage records retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<TokenUsageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchTokenUsage(
        [FromQuery] Guid? assistantId = null,
        [FromQuery] Guid? conversationId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] AIUsageType? usageType = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Token usage search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchTokenUsageQuery(assistantId, conversationId, userId, usageType, from, to, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<TokenUsageDto>>.SuccessResult(
            result.Value!, "Token usage records retrieved successfully."));
    }

    /// <summary>
    /// Gets an aggregated summary of token usage.
    /// </summary>
    /// <param name="assistantId">Filter by assistant</param>
    /// <param name="conversationId">Filter by conversation</param>
    /// <param name="userId">Filter by user</param>
    /// <param name="from">Start of the date range</param>
    /// <param name="to">End of the date range</param>
    /// <param name="usageType">Filter by usage type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The token usage summary</returns>
    /// <response code="200">Token usage summary retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<TokenUsageSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTokenUsageSummary(
        [FromQuery] Guid? assistantId = null,
        [FromQuery] Guid? conversationId = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] AIUsageType? usageType = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching token usage summary");

        var result = await _mediator.Send(
            new GetTokenUsageSummaryQuery(assistantId, conversationId, userId, from, to, usageType),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<TokenUsageSummaryDto>.SuccessResult(
            result.Value!, "Token usage summary retrieved successfully."));
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

        if (error.Contains("insufficient permissions", StringComparison.OrdinalIgnoreCase) ||
            error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
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
