namespace KhanHomeFloralLine.Application.Auth;

public sealed record RegisterRequest(string FullName, string Email, string Phone, string Password);
public sealed record LoginRequest(string Email, string Password);
public sealed record RefreshRequest(string RefreshToken);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc, string Role, string FullName);

