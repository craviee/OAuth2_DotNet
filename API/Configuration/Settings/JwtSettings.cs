namespace API.Configuration.Settings;

public class JwtSettings
{
    public string? SecurityKey { get; set; }
    public string? ValidIssuer { get; set; }
    public int? TokenLifetimeMinutes { get; set; }
}