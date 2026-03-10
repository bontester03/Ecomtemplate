using KhanHomeFloralLine.Application.Delivery;
using KhanHomeFloralLine.Domain.Entities;
using KhanHomeFloralLine.Infrastructure.Persistence;
using KhanHomeFloralLine.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Tests;

public class DeliveryServiceTests
{
    [Fact]
    public async Task ValidateAsync_ReturnsInvalid_AfterCutoffForSameDay()
    {
        await using var db = CreateDb();
        var zoneId = Guid.NewGuid();
        var slotId = Guid.NewGuid();

        db.DeliveryZones.Add(new DeliveryZone { Id = zoneId, Emirate = "Dubai", Area = "Marina", Charge = 20m, IsActive = true });
        db.DeliveryTimeSlots.Add(new DeliveryTimeSlot { Id = slotId, Label = "10-12", StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0), IsActive = true });
        db.AppSettings.Add(new AppSetting { Id = Guid.NewGuid(), Key = "SameDayCutoffHour", Value = "14" });
        await db.SaveChangesAsync();

        var service = new DeliveryService(db);
        var now = new DateTime(2026, 3, 10, 14, 30, 0, DateTimeKind.Utc);

        var result = await service.ValidateAsync(new DeliveryValidationRequest(zoneId, slotId, DateOnly.FromDateTime(now), now));

        Assert.False(result.IsValid);
        Assert.Contains("Same-day delivery unavailable", result.Error);
    }

    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}

