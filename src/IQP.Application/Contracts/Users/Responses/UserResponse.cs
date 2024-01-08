using IQP.Domain.Entities;

namespace IQP.Application.Contracts.Users.Responses;

public class UserResponse
{
    public UserResponse()
    {
    }
    
    public UserResponse(Guid id, string nickname, string email, UserStatus status)
    {
        Id = id;
        Nickname = nickname;
        Email = email;
        Status = status;
        // Registered = registered;
    }

    public required Guid Id { get; set; }
    public required string Nickname { get; set; }
    public required string Email { get; set; }
    public required UserStatus Status { get; set; }
    // public required DateTime Registered { get; set; }
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
            // Registered = user.Registered,
        };
    }
}