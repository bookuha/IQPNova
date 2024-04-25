using IQP.Domain.Entities;

namespace IQP.Web.ViewModels.Auth;

public class RegisterRequest
{
    public string Nickname { get; init; }
    public string Password { get; init; }
    public string Email { get; init; }
    public UserStatus Status { get; init; }
}