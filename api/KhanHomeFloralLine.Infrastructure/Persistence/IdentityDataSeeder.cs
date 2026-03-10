using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Infrastructure.Persistence;

public sealed class IdentityDataSeeder(
    UserManager<User> userManager,
    RoleManager<IdentityRole<Guid>> roleManager)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await EnsureRoleAsync(UserRole.Admin.ToString());
        await EnsureRoleAsync(UserRole.Staff.ToString());
        await EnsureRoleAsync(UserRole.Customer.ToString());

        var email = "admin@khanhomefloral.ae";
        var normalizedEmail = email.ToUpperInvariant();
        var admin = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

        if (admin is null)
        {
            admin = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                UserName = email,
                Email = email,
                PhoneNumber = "+971500000000",
                FullName = "Khan Admin",
                Role = UserRole.Admin,
                IsActive = true,
                EmailConfirmed = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            var create = await userManager.CreateAsync(admin, "Admin@123!");
            if (!create.Succeeded)
            {
                var errors = string.Join("; ", create.Errors.Select(x => x.Description));
                throw new InvalidOperationException($"Failed to seed admin user: {errors}");
            }
        }
        else
        {
            admin.Role = UserRole.Admin;
            admin.IsActive = true;
            admin.EmailConfirmed = true;
            admin.UserName ??= admin.Email;
            admin.NormalizedUserName ??= admin.UserName?.ToUpperInvariant();
            admin.NormalizedEmail ??= admin.Email?.ToUpperInvariant();
            admin.PhoneNumber ??= "+971500000000";
            admin.SecurityStamp ??= Guid.NewGuid().ToString("N");
            admin.ConcurrencyStamp ??= Guid.NewGuid().ToString("N");
            admin.PasswordHash = userManager.PasswordHasher.HashPassword(admin, "Admin@123!");
            admin.UpdatedAtUtc = DateTime.UtcNow;
            await userManager.UpdateAsync(admin);
        }

        if (!await userManager.IsInRoleAsync(admin, UserRole.Admin.ToString()))
        {
            await userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());
        }
    }

    private async Task EnsureRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName)
        {
            NormalizedName = roleName.ToUpperInvariant()
        });

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(x => x.Description));
            throw new InvalidOperationException($"Failed to seed role {roleName}: {errors}");
        }
    }
}
