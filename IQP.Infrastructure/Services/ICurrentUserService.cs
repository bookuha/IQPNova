using System.Security.Claims;

namespace IQP.Infrastructure.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    bool IsInRole(string role);
    public ClaimsPrincipal GetUser();
}