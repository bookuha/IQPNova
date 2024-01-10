using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using IQP.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HttpExceptionHandlingUtilities.WriteExceptionToContextAsync(context, ex);
        }
    }
}

public static class HttpExceptionHandlingUtilities
{
    public static async Task WriteExceptionToContextAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        switch (exception)
        {
            case ValidationException iqpvex:
            {
                context.Response.StatusCode = (int) ResolveHttpStatusCode(iqpvex);
                var validationProblemDetails = new ValidationProblemDetails(iqpvex.Errors)
                {
                    Type = $"{iqpvex.EntityName}.{iqpvex.Error}",
                    Title = iqpvex.Title,
                    Status = (int) ResolveHttpStatusCode(iqpvex),
                    Detail = exception.Message
                };

                var json = JsonSerializer.Serialize(validationProblemDetails);
                await context.Response.WriteAsync(json);
                break;
            }
            case IqpException iqpex:
            {
                context.Response.StatusCode = (int) ResolveHttpStatusCode(iqpex);
                var problemDetails = new ProblemDetails
                {
                    Status = (int) ResolveHttpStatusCode(iqpex),
                    Type = $"{iqpex.EntityName}.{iqpex.Error}",
                    Title = iqpex.Title,
                    Detail = exception.Message,
                };

                var json = JsonSerializer.Serialize(problemDetails);
                await context.Response.WriteAsync(json);
                break;
            }
            case AuthenticationException:
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                // New schemes can be added in future, but for now it is JWT only.
                context.Response.Headers.Add("WWW-Authenticate", JwtBearerDefaults.AuthenticationScheme);
                break;
            default:
            {
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
                break;
            }
        }
    }
    
    private static HttpStatusCode ResolveHttpStatusCode(IqpException exception)
    {
        // Make it more typed I guess. Solution 1) Move Errors to Domain. Check later if suitable (if all errors are domain based)
        return exception.Error switch
        {
            "AlreadyExists" => HttpStatusCode.Conflict,
            "NotFound" => HttpStatusCode.NotFound,
            "Validation" => HttpStatusCode.BadRequest,
            "WrongFlow" => HttpStatusCode.BadRequest,
            "Restricted" => HttpStatusCode.Forbidden,
            _ => HttpStatusCode.NotImplemented,

        };
    }
}