using System.Net.Mime;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1;

/// <summary>
/// Manages AI agents and their workflow bindings.
/// </summary>
[ApiController]
[Route("api/v1/ai/agents")]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize(Roles = "AI Administrator,System Admin")]
[Tags("AI Agents")]
public class AgentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AgentsController> _logger;

    public AgentsController(IMediator mediator, ILogger<AgentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new AI agent.
    /// </summary>
    /// <param name="command">Agent creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created agent</returns>
    /// <response code="200">Agent created successfully</response>
    /// <response code="400">Validation error</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAgent(
        [FromBody] CreateAgentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating agent: {AgentName}", command.Name);

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent created successfully."));
    }

    /// <summary>
    /// Searches agents with optional filters and pagination.
    /// </summary>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="agentType">Filter by agent type</param>
    /// <param name="workflowId">Filter by workflow</param>
    /// <param name="isActive">Filter by active status</param>
    /// <param name="page">Page number (1-based, default 1)</param>
    /// <param name="pageSize">Items per page (default 20, max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of agents</returns>
    /// <response code="200">Agents retrieved successfully</response>
    /// <response code="400">Validation error</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<AgentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchAgents(
        [FromQuery] string? searchTerm = null,
        [FromQuery] AIAgentType? agentType = null,
        [FromQuery] Guid? workflowId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Agent search: Page={Page}, PageSize={PageSize}", page, pageSize);

        var result = await _mediator.Send(
            new SearchAgentsQuery(searchTerm, agentType, workflowId, isActive, page, pageSize),
            cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<IReadOnlyList<AgentDto>>.SuccessResult(
            result.Value!, "Agents retrieved successfully."));
    }

    /// <summary>
    /// Gets an agent by its unique identifier.
    /// </summary>
    /// <param name="agentId">The agent's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The agent details</returns>
    /// <response code="200">Agent retrieved successfully</response>
    /// <response code="404">Agent not found</response>
    [HttpGet("{agentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAgentById(
        Guid agentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching agent: {AgentId}", agentId);

        var result = await _mediator.Send(new GetAgentByIdQuery(agentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent retrieved successfully."));
    }

    /// <summary>
    /// Updates an existing agent.
    /// </summary>
    /// <param name="agentId">The agent's unique identifier</param>
    /// <param name="command">Update details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated agent</returns>
    /// <response code="200">Agent updated successfully</response>
    /// <response code="404">Agent not found</response>
    [HttpPatch("{agentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAgent(
        Guid agentId,
        [FromBody] UpdateAgentCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating agent: {AgentId}", agentId);

        var result = await _mediator.Send(new UpdateAgentCommand(
            agentId,
            command.Name,
            command.Description,
            command.AgentType,
            command.SystemPrompt,
            command.Temperature,
            command.MaxIterations,
            command.MemoryEnabled,
            command.ModelId,
            command.ToolsJson,
            command.ExpectedRowVersion), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent updated successfully."));
    }

    /// <summary>
    /// Enables an agent.
    /// </summary>
    /// <param name="agentId">The agent's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The enabled agent</returns>
    /// <response code="200">Agent enabled successfully</response>
    /// <response code="404">Agent not found</response>
    [HttpPost("{agentId:guid}/enable")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> EnableAgent(
        Guid agentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Enabling agent: {AgentId}", agentId);

        var result = await _mediator.Send(new EnableAgentCommand(agentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent enabled successfully."));
    }

    /// <summary>
    /// Disables an agent.
    /// </summary>
    /// <param name="agentId">The agent's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The disabled agent</returns>
    /// <response code="200">Agent disabled successfully</response>
    /// <response code="404">Agent not found</response>
    [HttpPost("{agentId:guid}/disable")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DisableAgent(
        Guid agentId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Disabling agent: {AgentId}", agentId);

        var result = await _mediator.Send(new DisableAgentCommand(agentId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent disabled successfully."));
    }

    /// <summary>
    /// Assigns a workflow to an agent.
    /// </summary>
    /// <param name="agentId">The agent's unique identifier</param>
    /// <param name="command">Workflow assignment details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated agent</returns>
    /// <response code="200">Workflow assigned successfully</response>
    /// <response code="404">Agent not found</response>
    [HttpPut("{agentId:guid}/workflow")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignWorkflow(
        Guid agentId,
        [FromBody] AssignWorkflowCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning workflow to agent: {AgentId}", agentId);

        var result = await _mediator.Send(
            new AssignWorkflowCommand(agentId, command.WorkflowId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Workflow assigned successfully."));
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
