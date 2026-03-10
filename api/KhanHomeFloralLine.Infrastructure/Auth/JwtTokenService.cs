using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KhanHomeFloralLine.Infrastructure.Auth;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string GenerateAccessToken(User user, string role)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt:Key");
        var issuer = configuration["Jwt:Issuer"] ?? "KhanHomeFloralLine";
        var audience = configuration["Jwt:Audience"] ?? "KhanHomeFloralLine.Client";
        var expiryMinutes = int.TryParse(configuration["Jwt:AccessTokenMinutes"], out var value) ? value : 30;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, role)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
