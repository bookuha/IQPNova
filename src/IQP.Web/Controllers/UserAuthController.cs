using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using IQP.Application.Contracts.Users.Commands;
using IQP.Application.Services;
using IQP.Web.ViewModels;
using IQP.Web.ViewModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace IQP.Web.Controllers;

[Route("api/auth")]
[ApiController]
public class UserAuthController : ControllerBase
{
    private readonly IUserService _usersService;
    private readonly IConfiguration _configuration;

    public UserAuthController(IUserService usersService, IConfiguration configuration)
    {
        _usersService = usersService;
        _configuration = configuration;
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<AuthResult>> Register([FromBody] CreateUserCommand command) // TODO: Change to request
    {
        var user = await _usersService.Register(command);
        var token = await GenerateToken(user.Id);
        
        return Ok(new AuthResult
        {
            Token = token.Token,
            Expiration = token.Expiration,
            UserInfo = user
        });
    }
    
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request) // TODO: Change to request
    {
        var user = await _usersService.Login(request.Nickname, request.Password);
        var token = await GenerateToken(user.Id);
        
        return Ok(new AuthResult
        {
            Token = token.Token,
            Expiration = token.Expiration,
            UserInfo = user
        });
    }
    
    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    [Route("test")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public IActionResult Test()
    {
        return Ok(
            $"""
            You're authorized.
            Your id is {User.Claims.FirstOrDefault(c=>c.Type == ClaimTypes.NameIdentifier)?.Value ?? "Undefined"}
            Your name is {User.Identity?.Name ?? "Undefined"}
            """);
    }

    // If user has been successfully registered but the flow fails here, it won't be handled right now.
    // It can be possibly handler by introducing an exception that will inform user about what happened and ask to retry.
    private async Task<JwtResult> GenerateToken(Guid userId)
    {
        var claims = await _usersService.GetClaims(userId);

        if (claims is null)
        {
            throw new AuthenticationException(); // TODO: Change to normal
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));
        
        return new JwtResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        };
    }
    
}