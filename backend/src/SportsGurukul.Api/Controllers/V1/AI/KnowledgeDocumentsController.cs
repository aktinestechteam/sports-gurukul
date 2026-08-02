using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;

namespace SportsGurukul.Api.Controllers.V1.AI;

[Route("api/v1/knowledge-bases/{knowledgeBaseId:guid}/documents")]
[Authorize(Roles = "Platform Administrator,AI Administrator")]
[Tags("Knowledge Documents")]
public class KnowledgeDocumentsController : AIControllerBase
{
    public KnowledgeDocumentsController(IMediator mediator, ILogger<KnowledgeDocumentsController> logger)
        : base(mediator, logger)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<KnowledgeDocumentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocuments(
        Guid knowledgeBaseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("Fetching documents for knowledge base {KnowledgeBaseId}", knowledgeBaseId);

        var result = await Mediator.Send(new KnowledgeBaseQuery(knowledgeBaseId), cancellationToken);

        if (!result.IsSuccess)
            return HandleFailure(result.Error!);

        var documents = result.Value?.Sources?
            .SelectMany(s => Enumerable.Empty<KnowledgeDocumentDto>())
            .ToList() ?? [];

        return Ok(ApiResponse<List<KnowledgeDocumentDto>>.SuccessResult(documents, "Documents retrieved successfully."));
    }

    }
}
