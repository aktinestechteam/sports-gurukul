using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Api.Common.Models.AI;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/agents")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Agents")]
public class AgentsController : AIControllerBase
{
    public AgentsController(IMediator mediator, ILogger<AgentsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateAgentRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Creating agent: {Name}", request.Name);

        var command = new CreateAgentCommand(
            request.Name, request.Description, request.AssistantId,
            request.Configuration, request.Tools, request.Rules,
            request.Constraints, request.MaxIterations, request.RequiresApproval);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id, version = "1.0" },
            ApiResponse<AgentDto>.SuccessResult(result.Value, "Agent created successfully."));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AgentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] AgentStatus? status,
        [FromQuery] Guid? assistantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Searching agents");

        var query = new SearchAgentsQuery(searchTerm, status, assistantId, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<AgentSummaryDto>>.SuccessResult(
            result.Value!, "Agents retrieved successfully."));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<AgentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAlias(
        [FromQuery] string? searchTerm,
        [FromQuery] AgentStatus? status,
        [FromQuery] Guid? assistantId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        return await Search(searchTerm, status, assistantId, page, pageSize, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Fetching agent {AgentId}", id);

        var result = await Mediator.Send(new AgentQuery(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent retrieved successfully."));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAgentRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Updating agent {AgentId}", id);

        var command = new UpdateAgentCommand(
            id, request.Name, request.Description, request.Configuration,
            request.Tools, request.Rules, request.Constraints,
            request.MaxIterations, request.RequiresApproval);
        var result = await Mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent updated successfully."));
    }

    [HttpPost("{id:guid}/enable")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Enable(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Enabling agent {AgentId}", id);

        var result = await Mediator.Send(new EnableAgentCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent enabled successfully."));
    }

    [HttpPost("{id:guid}/disable")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Disable(
        Guid id,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Disabling agent {AgentId}", id);

        var result = await Mediator.Send(new DisableAgentCommand(id), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Agent disabled successfully."));
    }

    [HttpPost("{id:guid}/workflow")]
    [ProducesResponseType(typeof(ApiResponse<AgentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignWorkflow(
        Guid id,
        [FromBody] AssignWorkflowRequest request,
        CancellationToken cancellationToken)
    {
        Logger.LogInformation("Assigning workflow {WorkflowId} to agent {AgentId}", request.WorkflowDefinitionId, id);

        var result = await Mediator.Send(new AssignWorkflowCommand(id, request.WorkflowDefinitionId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<AgentDto>.SuccessResult(result.Value!, "Workflow assigned successfully."));
    }

    }
}
