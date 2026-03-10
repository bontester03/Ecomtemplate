using KhanHomeFloralLine.Application.Abstractions;
using KhanHomeFloralLine.Application.Delivery;
using KhanHomeFloralLine.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KhanHomeFloralLine.Infrastructure.Services;

public class DeliveryService(IAppDbContext dbContext) : IDeliveryService
{
    public async Task<DeliveryValidationResult> ValidateAsync(DeliveryValidationRequest request, CancellationToken cancellationToken = default)
    {
        var zone = await dbContext.DeliveryZones.FirstOrDefaultAsync(x => x.Id == request.DeliveryZoneId && x.IsActive, cancellationToken);
        if (zone is null)
        {
            return new DeliveryValidationResult(false, 0, "Invalid delivery zone");
        }

        var slot = await dbContext.DeliveryTimeSlots.FirstOrDefaultAsync(x => x.Id == request.DeliveryTimeSlotId && x.IsActive, cancellationToken);
        if (slot is null)
        {
            return new DeliveryValidationResult(false, 0, "Invalid delivery time slot");
        }

        if (request.DeliveryDate < DateOnly.FromDateTime(request.NowUtc))
        {
            return new DeliveryValidationResult(false, 0, "Delivery date cannot be in the past");
        }

        var cutoffSetting = await dbContext.AppSettings.FirstOrDefaultAsync(x => x.Key == "SameDayCutoffHour", cancellationToken);
        var cutoffHour = cutoffSetting is null ? 14 : int.Parse(cutoffSetting.Value);

        if (request.DeliveryDate == DateOnly.FromDateTime(request.NowUtc) && request.NowUtc.Hour >= cutoffHour)
        {
            return new DeliveryValidationResult(false, 0, $"Same-day delivery unavailable after {cutoffHour}:00");
        }

        if (slot.CapacityLimit.HasValue && slot.CapacityLimit.Value > 0)
        {
            var bookedCount = await dbContext.Orders.CountAsync(
                x => x.DeliveryDate == request.DeliveryDate
                     && x.DeliveryTimeSlotId == slot.Id
                     && x.Status != OrderStatus.Cancelled
                     && x.Status != OrderStatus.Refunded,
                cancellationToken);

            if (bookedCount >= slot.CapacityLimit.Value)
            {
                return new DeliveryValidationResult(false, 0, "Selected delivery slot is full");
            }
        }

        return new DeliveryValidationResult(true, zone.Charge, null);
    }
}
