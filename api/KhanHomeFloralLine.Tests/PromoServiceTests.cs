using KhanHomeFloralLine.Application.Promotions;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Domain.Enums;
using KhanHomeFloralLine.Infrastructure.Persistence;
using KhanHomeFloralLine.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Tests;

public class PromoServiceTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsDiscount_WhenPromoValid()
    {
        await using var db = CreateDb();
        var promo = new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = "SAVE20",
            Type = PromoType.Percentage,
            Value = 20m,
            MinSpend = 100m,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
            IsActive = true
        };
        db.PromoCodes.Add(promo);
        await db.SaveChangesAsync();

        var service = new PromoService(db);
        var result = await service.ValidateAsync(new PromoValidationRequest("SAVE20", 200m, DateTime.UtcNow));

        Assert.True(result.IsValid);
        Assert.Equal(40m, result.DiscountAmount);
    }

    [Fact]
    public async Task ValidateAsync_ReturnsInvalid_WhenExpired()
    {
        await using var db = CreateDb();
        db.PromoCodes.Add(new PromoCode
        {
            Id = Guid.NewGuid(),
            Code = "OLD10",
            Type = PromoType.FixedAmount,
            Value = 10m,
            MinSpend = 50m,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(-1),
            IsActive = true
        });
        await db.SaveChangesAsync();

        var service = new PromoService(db);
        var result = await service.ValidateAsync(new PromoValidationRequest("OLD10", 200m, DateTime.UtcNow));

        Assert.False(result.IsValid);
        Assert.Equal("Promo code is expired", result.Error);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}

