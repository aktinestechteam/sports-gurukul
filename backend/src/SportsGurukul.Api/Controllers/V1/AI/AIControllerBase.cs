using System.Net.Mime;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace SportsGurukul.Api.Controllers.V1.AI;

[ApiController]
[ApiVersion("1.0")]
[Produces(MediaTypeNames.Application.Json)]
public abstract class AIControllerBase : ControllerBase
{
    protected readonly IMediator Mediator;
    protected readonly ILogger Logger;

    protected AIControllerBase(IMediator mediator, ILogger logger)
    {
        Mediator = mediator;
        Logger = logger;
    }

    protected Guid? GetUserId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim is not null && Guid.TryParse(claim, out var id) ? id : null;
    }

    protected IActionResult HandleFailure(string error)
    {
        if (error.Contains("not found", StringComparison.OrdinalIgnoreCase))
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = error,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            });

        return BadRequest(new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Bad Request",
            Detail = error,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        });
    }
}
