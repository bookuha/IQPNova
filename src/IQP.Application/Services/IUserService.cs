using System.Security.Claims;
using IQP.Application.Contracts.Users.Commands;
using IQP.Application.Contracts.Users.Responses;

namespace IQP.Application.Services;

public interface IUserService
{
    public Task<UserResponse> Register(CreateUserCommand command);
    public Task<UserResponse> Login(string nickname, string password);
    public Task<IEnumerable<Claim>?> GetClaims(Guid userId);
}