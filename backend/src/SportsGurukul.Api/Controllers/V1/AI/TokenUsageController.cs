using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/token-usage")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Token Usage")]
public class TokenUsageController : AIControllerBase
{
    public TokenUsageController(IMediator mediator, ILogger<TokenUsageController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<TokenUsageSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsage(
        [FromQuery] Guid? conversationId,
        [FromQuery] string? userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Fetching token usage");

        var query = new TokenUsageQuery(conversationId, userId, fromDate, toDate, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<TokenUsageSummaryDto>>.SuccessResult(
            result.Value!, "Token usage retrieved successfully."));
    }
}
