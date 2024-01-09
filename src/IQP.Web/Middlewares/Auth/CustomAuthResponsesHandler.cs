using IQP.Application;
using IQP.Domain;
using IQP.Domain.Exceptions;
using IQP.Web.Middlewares.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace IQP.Web.Middlewares;

public class CustomAuthResponsesHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        
        if (authorizeResult.Forbidden
            && authorizeResult.AuthorizationFailure!.FailedRequirements
                .OfType<AdminOnlyRequirement>().Any())
        {
            var exception = new IqpException(
                EntityName.User, Errors.Restricted.ToString(), "Forbidden",
                "You are not allowed to access this resource."); // TODO: Maybe any better naming/code?
            
            await HttpExceptionHandlingUtilities.WriteExceptionToContextAsync(context, exception);
            return;
        }

        // Fall back to the default implementation.
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}

