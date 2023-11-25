namespace IQP.Web.ViewModels;

public class JwtResult
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    // TODO: public string RefreshToken { get; set; }
}