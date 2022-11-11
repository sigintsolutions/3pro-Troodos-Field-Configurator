namespace Thingsboard.Auth;

public class ParsedJWTFormat
{
#pragma warning disable IDE1006 // Naming Styles
    public string? sub { get; set; }
    public string[]? scopes { get; set; }
    public string? userId { get; set; }
    public string? firstName { get; set; }
    public string? lastName { get; set; }
    public bool enabled { get; set; }
    public bool isPublic { get; set; }
    public string? tenantId { get; set; }
    public string? customerId { get; set; }
    public string? iss { get; set; }
    public int iat { get; set; }
    public int exp { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
