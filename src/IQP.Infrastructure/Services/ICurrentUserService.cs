using System.Security.Claims;

namespace IQP.Infrastructure.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; set; } // "set;" only for testing purposes
    public bool IsAuthenticated { get; }
    bool IsInRole(string role);
    public ClaimsPrincipal GetUser();
}