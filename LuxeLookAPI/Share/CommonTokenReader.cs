using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CommonTokenReader
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CommonTokenReader(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public (Guid? UserId, string? RoleName, string? UserName) GetUserFromContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null || !httpContext.Request.Headers.ContainsKey("Authorization"))
            return (null, null, null);

        // Extract token and validate
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (!authHeader.StartsWith("Bearer "))
            return (null, null, null);

        var token = authHeader.Substring("Bearer ".Length).Trim();
        if (string.IsNullOrEmpty(token)) return (null, null, null);

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = principal.FindFirstValue(ClaimTypes.Role);
            var userName = principal.FindFirstValue(ClaimTypes.Name);

            return (
                string.IsNullOrEmpty(userId) ? null : Guid.Parse(userId),
                role,
                userName
            );
        }
        catch
        {
            return (null, null, null);
        }
    }
}
