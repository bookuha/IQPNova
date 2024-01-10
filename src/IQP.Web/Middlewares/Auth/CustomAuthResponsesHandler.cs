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
            await HttpExceptionHandlingUtilities.WriteExceptionToContextAsync(context, IqpException.NotAdmin());
            return;
        }

        // Fall back to the default implementation.
        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}

