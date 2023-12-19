using IQP.Application.Contracts.Users.Responses;

namespace IQP.Web.ViewModels;

public class AuthResult
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public UserResponse UserInfo { get; set; }
    // TODO: public string RefreshToken { get; set; }
}