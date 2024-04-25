using IQP.Domain.Entities;

namespace IQP.Application.Services.Users;

public record UserResponse
{
    public required Guid Id { get; init; }
    public required string Nickname { get; init; }
    public required string Email { get; init; }
    public required UserStatus Status { get; init; }
    public required bool IsAdmin { get; init; }
}

public static partial class UserMappingExtensions{
    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Nickname = user.UserName,
            Email = user.Email,
            Status = user.Status,
            IsAdmin = user.IsAdmin
        };
    }
}