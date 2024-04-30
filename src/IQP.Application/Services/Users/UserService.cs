using System.Security.Claims;
using IQP.Domain;
using IQP.Domain.Entities;
using IQP.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace IQP.Application.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<User> userManager, ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    
    public async Task<UserResponse> Register(string nickname, string password, string email, UserStatus status)
    {
        var user = new User
        {
            UserName = nickname,
            Email = email,
            Status = status
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed");
            throw new ValidationException(EntityName.User,
                result.Errors.ToDictionary(e => e.Code, e => new[] {e.Description})); // Is this right?
        }

        _logger.LogInformation("User created a new account: {Nickname}, {Email}, {Status}", user.UserName, user.Email,
            user.Status);

        return user.ToResponse();
    }

    public async Task<UserResponse> Login(string nickname, string password) // TODO: Make command later
    {
        var user = await _userManager.FindByNameAsync(nickname);

        if (user is null)
        {
            throw new IqpException(
                EntityName.User, Errors.NotFound.ToString(), "User not found",
                "The user with such nickname does not exist.");
        }

        var result = await _userManager.CheckPasswordAsync(user, password);

        if (!result)
        {
            throw new IqpException(
                EntityName.User, Errors.Restricted.ToString(), "Wrong password",
                "The supplied password is wrong.");
        }

        _logger.LogInformation("User logged in: {Nickname}, {Email}, {Status}", user.UserName, user.Email, user.Status);

        return user.ToResponse();
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user;
    }

    public async Task<IEnumerable<Claim>?> GetClaims(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return null;
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(
                "Admin",
                user.IsAdmin.ToString().ToLower(),
                ClaimValueTypes.Boolean
                )
        };

        var roles = await _userManager.GetRolesAsync(user);

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
    
    public async Task<bool> IsUserAdmin(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            throw new IqpException(
                EntityName.User, Errors.NotFound.ToString(), "User not found",
                "The user with such id does not exist.");
        }

        return user.IsAdmin;
    }
}