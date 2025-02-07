namespace API.Modules.User;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }
    
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Create a new user object
        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        // Add the user to the database and set their password
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            return Ok(new { message = "User created successfully" });
        }

        // Handle errors (e.g., username already exists, password too weak, etc.)
        return BadRequest(new { errors = result.Errors });
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Find user by username
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong");

        var now = DateTime.UtcNow;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };
        
        var token = new JwtSecurityToken(
            issuer: "app", // Optional
            audience: null, // Optional
            claims: claims,
            notBefore: null, // Avoid setting "nbf"
            expires: null, // Avoid setting "exp"
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );
        
        // var tokenDescriptor = new SecurityTokenDescriptor
        // {
        //     Subject = new ClaimsIdentity(new[]
        //     {
        //         new Claim(ClaimTypes.NameIdentifier, user.Id),
        //         new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        //     }),
        //     Issuer = "app",
        //     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        // };

        // var token = tokenHandler.CreateToken(tokenDescriptor);
        
        Console.WriteLine("Generated Token: " + tokenHandler.WriteToken(token));

        return Ok(new
        {
            token = tokenHandler.WriteToken(token),
            // expiration = token.ValidTo
        });
    }
}