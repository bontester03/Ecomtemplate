using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Orders;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Infrastructure.Auth;
using KhanHomeFloralLine.Infrastructure.Payments;
using KhanHomeFloralLine.Infrastructure.Persistence;
using KhanHomeFloralLine.Infrastructure.Services;
using KhanHomeFloralLine.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KhanHomeFloralLine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = ConvertDatabaseUrl(Environment.GetEnvironmentVariable("DATABASE_URL"))
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string not found");
        
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IOrderNumberGenerator, OrderNumberGenerator>();
        services.AddScoped<IPromoService, PromoService>();
        services.AddScoped<IDeliveryService, DeliveryService>();
        services.AddScoped<IOrderPricingService, OrderPricingService>();
        services.AddScoped<IBlobStorageService, LocalBlobStorageService>();
        services.AddScoped<IPaymentGatewayService, MockPaymentGatewayService>();
        services.AddScoped<IdentityDataSeeder>();
        services.AddScoped<CatalogDataSeeder>();

        return services;
    }

    private static string? ConvertDatabaseUrl(string? databaseUrl)
    {
        if (string.IsNullOrEmpty(databaseUrl)) return null;

        // Already in Npgsql format (has Host=)
        if (databaseUrl.Contains("Host=", StringComparison.OrdinalIgnoreCase)) return databaseUrl;

        // Convert postgresql://user:pass@host:port/db to Npgsql format
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        return $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Prefer;Trust Server Certificate=true";
    }
}

