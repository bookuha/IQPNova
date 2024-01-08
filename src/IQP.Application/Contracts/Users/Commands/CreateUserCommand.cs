using IQP.Domain.Entities;

namespace IQP.Application.Contracts.Users.Commands;

public class CreateUserCommand
{
    public CreateUserCommand()
    {
    }

    public CreateUserCommand(string nickname, string email, string password, UserStatus userStatus)
    {
        Nickname = nickname;
        Email = email;
        Password = password;
        Status = userStatus;
    }

    public required string Nickname { get; set;}
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required UserStatus Status { get; set; }
}