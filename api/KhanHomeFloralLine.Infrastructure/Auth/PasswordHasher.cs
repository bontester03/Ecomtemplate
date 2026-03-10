using KhanHomeFloralLine.Application.Abstractions;

namespace KhanHomeFloralLine.Infrastructure.Auth;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string raw) => BCrypt.Net.BCrypt.HashPassword(raw);
    public bool Verify(string raw, string hash) => BCrypt.Net.BCrypt.Verify(raw, hash);
}

