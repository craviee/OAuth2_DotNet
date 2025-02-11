using System.Configuration;
using API.Configuration.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace API.Modules.User;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserManager<IdentityUser> userManager, IOptions<JwtSettings> jwtSettings)
    : ControllerBase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email
        };
        
        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok(new { message = "User created successfully" });
        }
        
        return BadRequest(new { errors = result.Errors });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Find user by username
        var user = await userManager.FindByNameAsync(request.Username);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings?.SecurityKey ?? throw new InvalidConfigurationException());

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };
        
        var token = new JwtSecurityToken(
            issuer: _jwtSettings.ValidIssuer,
            audience: null,
            claims: claims,
            notBefore: null,
            expires: null,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return Ok(new
        {
            token = tokenHandler.WriteToken(token),
        });
    }
}