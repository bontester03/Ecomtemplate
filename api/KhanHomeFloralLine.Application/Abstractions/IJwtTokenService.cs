using KhanHomeFloralLine.Domain.Entities;

namespace KhanHomeFloralLine.Application.Abstractions;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user, string role);
    string GenerateRefreshToken();
}
