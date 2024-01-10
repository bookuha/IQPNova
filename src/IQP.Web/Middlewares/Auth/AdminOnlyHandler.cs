using Microsoft.AspNetCore.Authorization;

namespace IQP.Web.Middlewares.Auth;

public class AdminOnlyHandler : AuthorizationHandler<AdminOnlyRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, AdminOnlyRequirement requirement)
    {
        var adminClaim = context.User.FindFirst(
            c => c.Type == "Admin");

        if (adminClaim is null)
        {
            return Task.CompletedTask;
        }

        var isAdmin = Convert.ToBoolean(adminClaim.Value);
        Console.WriteLine(isAdmin);
        if (isAdmin)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class AdminOnlyRequirement : IAuthorizationRequirement { }