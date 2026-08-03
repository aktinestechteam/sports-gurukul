using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides AI model catalog discovery and routing candidate selection.
/// </summary>
[ApiController]
[Route("api/v1/ai/models")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Model Catalog")]
public class ModelCatalogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ModelCatalogController> _logger;

    public ModelCatalogController(IMediator mediator, ILogger<ModelCatalogController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lists models from the catalog with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="family">Filter by model family</param>
    /// <param name="providerId">Filter by provider</param>
    /// <param name="supportsChat">Filter by chat support</param>
    /// <param name="supportsFunctionCalling">Filter by function calling support</param>
    /// <param name="supportsVision">Filter by vision support</param>
    /// <param name="supportsJsonMode">Filter by JSON mode support</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 50, max 200)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of model candidates</returns>
    /// <response code="200">Models retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ModelCandidate>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListModels(
        [FromQuery] string? searchTerm = null,
        [FromQuery] AIModelFamily? family = null,
        [FromQuery] Guid? providerId = null,
        [FromQuery] bool? supportsChat = null,
        [FromQuery] bool? supportsFunctionCalling = null,
        [FromQuery] bool? supportsVision = null,
        [FromQuery] bool? supportsJsonMode = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Model catalog listing: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(new ListModelsQuery(
            searchTerm,
            family,
            providerId,
            supportsChat,
            supportsFunctionCalling,
            supportsVision,
            supportsJsonMode,
            page,
            pageSize), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<ModelCandidate>>.SuccessResult(
            result.Value!, "Models retrieved successfully."));
    }

    /// <summary>
    /// Gets models eligible for routing based on a request context.
    /// </summary>
    /// <param name="routingStrategy">Routing strategy (default Balanced)</param>
    /// <param name="assistantId">Optional assistant context</param>
    /// <param name="agentDefinitionId">Optional agent context</param>
    /// <param name="conversationId">Optional conversation context</param>
    /// <param name="estimatedInputTokens">Estimated input token count</param>
    /// <param name="maxOutputTokens">Maximum output token count</param>
    /// <param name="requiresFunctionCalling">Whether function calling is required</param>
    /// <param name="requiresVision">Whether vision support is required</param>
    /// <param name="requiresJsonMode">Whether JSON mode is required</param>
    /// <param name="maxCostPerRequest">Maximum cost per request</param>
    /// <param name="maxLatencyMs">Maximum latency in milliseconds</param>
    /// <param name="preferredModelIds">Preferred model ids</param>
    /// <param name="fallbackModelIds">Fallback model ids</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of eligible model candidates</returns>
    /// <response code="200">Available models retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<ModelCandidate>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableModels(
        [FromQuery] AIRoutingStrategy routingStrategy = AIRoutingStrategy.Balanced,
        [FromQuery] Guid? assistantId = null,
        [FromQuery] Guid? agentDefinitionId = null,
        [FromQuery] Guid? conversationId = null,
        [FromQuery] int? estimatedInputTokens = null,
        [FromQuery] int? maxOutputTokens = null,
        [FromQuery] bool requiresFunctionCalling = false,
        [FromQuery] bool requiresVision = false,
        [FromQuery] bool requiresJsonMode = false,
        [FromQuery] decimal? maxCostPerRequest = null,
        [FromQuery] int? maxLatencyMs = null,
        [FromQuery] IReadOnlyList<Guid>? preferredModelIds = null,
        [FromQuery] IReadOnlyList<Guid>? fallbackModelIds = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resolving available models for strategy: {RoutingStrategy}", routingStrategy);

        var result = await _mediator.Send(new GetAvailableModelsQuery(
            routingStrategy,
            assistantId,
            agentDefinitionId,
            conversationId,
            estimatedInputTokens,
            maxOutputTokens,
            requiresFunctionCalling,
            requiresVision,
            requiresJsonMode,
            maxCostPerRequest,
            maxLatencyMs,
            preferredModelIds,
            fallbackModelIds), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<ModelCandidate>>.SuccessResult(
            result.Value!, "Available models retrieved successfully."));
    }

    /// <summary>
    /// Gets a model candidate by its unique identifier.
    /// </summary>
    /// <param name="modelId">The model's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The model candidate details</returns>
    /// <response code="200">Model retrieved successfully</response>
    /// <response code="404">Model not found</response>
    [HttpGet("{modelId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ModelCandidate>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetModelById(
        Guid modelId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching model: {ModelId}", modelId);

        var result = await _mediator.Send(new GetModelByIdQuery(modelId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<ModelCandidate>.SuccessResult(result.Value!, "Model retrieved successfully."));
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
