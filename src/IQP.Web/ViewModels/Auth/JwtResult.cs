using IQP.Application.Services.Users;

namespace IQP.Web.ViewModels.Auth;

public class JwtResult
{
    public required string Token { get; init; }
    public DateTime Expiration { get; init; }
    public UserResponse UserInfo { get; set; }
    // TODO: public string RefreshToken { get; set; }
}