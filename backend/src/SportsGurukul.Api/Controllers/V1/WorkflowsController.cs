using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Provides AI workflow discovery and retrieval.
/// </summary>
[ApiController]
[Route("api/v1/ai/workflows")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "Coach,Athlete,Academy Admin,AI Administrator,System Admin")]
[Tags("AI Workflows")]
public class WorkflowsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(IMediator mediator, ILogger<WorkflowsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Searches workflows with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="workflowType">Filter by workflow type</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="isPublished">Filter by published status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of workflows</returns>
    /// <response code="200">Workflows retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WorkflowDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchWorkflows(
        [FromQuery] string? searchTerm = null,
        [FromQuery] AIWorkflowType? workflowType = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isPublished = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Workflow search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchWorkflowsQuery(searchTerm, workflowType, isActive, isPublished, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<WorkflowDto>>.SuccessResult(
            result.Value!, "Workflows retrieved successfully."));
    }

    /// <summary>
    /// Gets all published workflows.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of published workflows</returns>
    /// <response code="200">Published workflows retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet("published")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<WorkflowDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPublishedWorkflows(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching published workflows");

        var result = await _mediator.Send(new GetPublishedWorkflowsQuery(), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<WorkflowDto>>.SuccessResult(
            result.Value!, "Published workflows retrieved successfully."));
    }

    /// <summary>
    /// Gets a workflow by its unique identifier.
    /// </summary>
    /// <param name="workflowId">The workflow's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The workflow details</returns>
    /// <response code="200">Workflow retrieved successfully</response>
    /// <response code="404">Workflow not found</response>
    [HttpGet("{workflowId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<WorkflowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWorkflowById(
        Guid workflowId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching workflow: {WorkflowId}", workflowId);

        var result = await _mediator.Send(new GetWorkflowByIdQuery(workflowId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<WorkflowDto>.SuccessResult(result.Value!, "Workflow retrieved successfully."));
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
