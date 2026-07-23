using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SportsGurukul.Api.Common.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ApiExceptionFilterAttribute> _logger;
    private readonly IDictionary<Type, Func<ExceptionContext, IActionResult>> _exceptionHandlers;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type, Func<ExceptionContext, IActionResult>>
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(ConflictException), HandleConflictException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            context.Result = handler(context);
            context.ExceptionHandled = true;
            return;
        }

        HandleGenericException(context);
    }

    private IActionResult HandleValidationException(ExceptionContext context)
    {
        var exception = (ValidationException)context.Exception;

        var errors = exception.Errors
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(
                group => group.Key,
                group => group.ToArray());

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };

        problemDetails.Extensions["errors"] = errors;

        _logger.LogWarning("Validation error: {Errors}", string.Join(", ", exception.Errors.Select(e => e.ErrorMessage)));

        context.Result = new BadRequestObjectResult(problemDetails);
        return context.Result;
    }

    private IActionResult HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Title = "Unauthorized",
            Detail = "You are not authorized to access this resource.",
            Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
        };

        context.Result = new UnauthorizedObjectResult(problemDetails);
        return context.Result;
    }

    private IActionResult HandleForbiddenAccessException(ExceptionContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Forbidden,
            Title = "Forbidden",
            Detail = "You do not have permission to access this resource.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3"
        };

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = (int)HttpStatusCode.Forbidden
        };

        return context.Result;
    }

    private IActionResult HandleNotFoundException(ExceptionContext context)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = "Not Found",
            Detail = "The requested resource was not found.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        };

        context.Result = new NotFoundObjectResult(problemDetails);
        return context.Result;
    }

    private IActionResult HandleConflictException(ExceptionContext context)
    {
        var exception = (ConflictException)context.Exception;

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.Conflict,
            Title = "Conflict",
            Detail = exception.Message,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"
        };

        context.Result = new ConflictObjectResult(problemDetails);
        return context.Result;
    }

    private void HandleGenericException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred");

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "Internal Server Error",
            Detail = "An unexpected error occurred. Please try again later.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException(string message = "Access denied.") : base(message) { }
}

public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}
