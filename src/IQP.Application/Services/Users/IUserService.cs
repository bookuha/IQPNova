using System.Security.Claims;
using IQP.Domain.Entities;

namespace IQP.Application.Services.Users;

public interface IUserService
{
    public Task<UserResponse> Register(string nickname, string password, string email, UserStatus status);
    public Task<UserResponse> Login(string nickname, string password);
    public Task<IEnumerable<Claim>?> GetClaims(Guid userId);
    public Task<bool> IsUserAdmin(Guid userId);
}