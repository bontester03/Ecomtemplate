using System.IdentityModel.Tokens.Jwt;
using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Auth;
using KhanHomeFloralLine.Application.Common;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public class AuthController(
    IAppDbContext dbContext,
    IJwtTokenService jwtTokenService,
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.Phone?.Trim() ?? string.Empty;
        var exists = await userManager.FindByEmailAsync(email);
        if (exists is not null)
        {
            throw new AppValidationException("Email already registered");
        }

        await EnsureRoleExistsAsync(UserRole.Customer.ToString());

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            PhoneNumber = phone,
            FullName = request.FullName.Trim(),
            Role = UserRole.Customer,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new AppValidationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, UserRole.Customer.ToString());

        var refreshToken = CreateRefreshToken(user.Id);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await BuildAuthResponse(user, refreshToken.Token));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await userManager.FindByEmailAsync(email) ?? throw new AppValidationException("Invalid credentials");

        if (!user.IsActive)
        {
            throw new AppValidationException("User is inactive");
        }

        var validPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!validPassword)
        {
            throw new AppValidationException("Invalid credentials");
        }

        // Backfill: attach past guest orders to this customer account by email.
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            var guestOrders = await dbContext.Orders
                .Where(x => x.UserId == null && x.Email == user.Email)
                .ToListAsync(cancellationToken);

            foreach (var order in guestOrders)
            {
                order.UserId = user.Id;
                order.UpdatedAtUtc = DateTime.UtcNow;
            }
        }

        var refreshToken = CreateRefreshToken(user.Id);
        dbContext.RefreshTokens.Add(refreshToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await BuildAuthResponse(user, refreshToken.Token));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken && !x.IsRevoked, cancellationToken)
            ?? throw new AppValidationException("Invalid refresh token");

        if (token.ExpiresAtUtc < DateTime.UtcNow || token.User is null)
        {
            throw new AppValidationException("Refresh token expired");
        }

        token.IsRevoked = true;

        var newRefresh = CreateRefreshToken(token.UserId);
        dbContext.RefreshTokens.Add(newRefresh);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(await BuildAuthResponse(token.User, newRefresh.Token));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(RefreshRequest request, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);
        if (token is not null)
        {
            token.IsRevoked = true;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return NoContent();
    }

    [HttpPost("admin/users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateStaff(RegisterRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var phone = request.Phone?.Trim() ?? string.Empty;
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            throw new AppValidationException("Email already registered");
        }

        await EnsureRoleExistsAsync(UserRole.Staff.ToString());

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            PhoneNumber = phone,
            FullName = request.FullName.Trim(),
            Role = UserRole.Staff,
            IsActive = true,
            EmailConfirmed = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        var createResult = await userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            throw new AppValidationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, UserRole.Staff.ToString());
        return Ok();
    }

    private async Task EnsureRoleExistsAsync(string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }

    private RefreshToken CreateRefreshToken(Guid userId) => new()
    {
        UserId = userId,
        Token = jwtTokenService.GenerateRefreshToken(),
        ExpiresAtUtc = DateTime.UtcNow.AddDays(14)
    };

    private async Task<AuthResponse> BuildAuthResponse(User user, string refreshToken)
    {
        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? user.Role.ToString();
        var accessToken = jwtTokenService.GenerateAccessToken(user, role);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(accessToken);
        var expires = token.ValidTo;

        return new AuthResponse(accessToken, refreshToken, expires, role, user.FullName);
    }
}
