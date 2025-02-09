namespace API.Configuration.Settings;

public class JwtSettings
{
    public string JwtSecurityKey { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
}