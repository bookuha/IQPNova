using System.Net;
using System.Text.Json;
using IQP.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace IQP.Web.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (IqpException ex)
        {
            await HandleExpectedExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnexpectedExceptionAsync(context, ex);
        }
    }

    private async Task HandleExpectedExceptionAsync(HttpContext context, IqpException e)
    {
        _logger.LogError(e, e.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) ResolveHttpStatusCode(e);

        if (e is ValidationException ve)
        {
            var validationProblemDetails = new ValidationProblemDetails(ve.Errors)
            {
                Type = $"{e.EntityName}.{e.Error}",
                Title = e.Title,
                Status = (int) ResolveHttpStatusCode(e),
                Detail = e.Message
            };

            var json = JsonSerializer.Serialize(validationProblemDetails);
            await context.Response.WriteAsync(json);
        }
        else
        {
            var problemDetails = new ProblemDetails
            {
                Status = (int) ResolveHttpStatusCode(e),
                Type = $"{e.EntityName}.{e.Error}",
                Title = e.Title,
                Detail = e.Message,
            };

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json);
        }
    }

    private async Task HandleUnexpectedExceptionAsync(HttpContext context, Exception e)
    {
        _logger.LogError(e, e.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

        ProblemDetails problem = new()
        {
            Status = (int) HttpStatusCode.InternalServerError,
            Type = "InternalServerError",
            Title = "Internal server error.",
            Detail = "A critical internal server error occurred."
        };

        var json = JsonSerializer.Serialize(problem);

        await context.Response.WriteAsync(json);
    }

    public static HttpStatusCode ResolveHttpStatusCode(IqpException exception)
    {
        // Make it more typed I guess. Solution 1) Move Errors to Domain. Check later if suitable (if all errors are domain based)
        return exception.Error switch
        {
            "AlreadyExists" => HttpStatusCode.Conflict,
            "NotFound" => HttpStatusCode.NotFound,
            "Validation" => HttpStatusCode.BadRequest,
            "WrongFlow" => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.NotImplemented,

        };
    }
}