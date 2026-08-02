using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/models")]
[Authorize]
[Tags("Model Catalog")]
public class ModelCatalogController : AIControllerBase
{
    public ModelCatalogController(IMediator mediator, ILogger<ModelCatalogController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<ModelCatalogDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetModels(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? providerId,
        [FromQuery] string? capability,
        [FromQuery] bool? activeOnly,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Fetching model catalog");

        var query = new GetModelsQuery(searchTerm, providerId, capability, activeOnly, page, pageSize);
        var result = await Mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        return Ok(ApiResponse<PaginatedResult<ModelCatalogDto>>.SuccessResult(
            result.Value!, "Models retrieved successfully."));
    }
}
