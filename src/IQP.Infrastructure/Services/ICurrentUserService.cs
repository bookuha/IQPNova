using System.Security.Claims;

namespace IQP.Infrastructure.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    public bool IsAuthenticated { get; }
    bool IsInRole(string role);
    public ClaimsPrincipal GetUser();
}